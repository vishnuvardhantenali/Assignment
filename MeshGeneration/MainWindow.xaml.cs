using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MeshGeneration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent ();            
        }

        private void SquareMesh()
        {
            // Parameters
            double squareX = 550; 
            double squareY = 10; 
            double squareSize = 200;

            // Number of rings and segments
            int quadRowsCols;

            if (!int.TryParse (txtBoxzVerticles.Text.Trim (), out quadRowsCols) || quadRowsCols <= 0)
            {
                MessageBox.Show ("Enter a valid number of quadrilateral segments.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int triSegments;

            if (!int.TryParse (txtBoxTriSegments.Text.Trim (), out triSegments) || triSegments <= 0)
            {
                MessageBox.Show ("Enter a valid number of triangle segments.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Draw square meshes
            DrawSquareQuadMesh (squareX, squareY, squareSize, quadRowsCols);
            DrawSquareTriMesh (squareX, squareY + squareSize + 50, squareSize, triSegments);
        }

        private void CircleMesh()
        {
            // Parameters
            double circleCenterX = 150;
            double circleCenterY = 110;
            double circleRadius = 100;

            // Number of rings and segments
            int quadRowsCols;

            if (!int.TryParse (txtBoxzVerticles.Text.Trim (), out quadRowsCols) || quadRowsCols <= 0)
            {
                MessageBox.Show ("Enter a valid number of quadrilateral segments.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int triSegments;

            if (!int.TryParse (txtBoxTriSegments.Text.Trim (), out triSegments) || triSegments <= 0)
            {
                MessageBox.Show ("Enter a valid number of triangle segments.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Draw circle meshes
            DrawCircleQuadraMesh (circleCenterX, circleCenterY, circleRadius, quadRowsCols);
            DrawCircleTriangleMesh (circleCenterX, circleCenterY + circleRadius * 2 + 50, circleRadius, triSegments);
        }

        // 1. Quadrilateral Mesh on Square
        private void DrawSquareQuadMesh(double startX, double startY, double size, int divisions)
        {
            double cellSize = size / divisions;

            // Draw outer square boundary
            DrawRectangle (startX, startY, size, size, Brushes.Black, 2);

            // Draw grid lines (horizontal and vertical)
            for (int i = 1; i < divisions; i++)
            {
                // Horizontal
                DrawLine (new Point (startX, startY + i * cellSize), new Point (startX + size, startY + i * cellSize), Brushes.Gray);
                // Vertical
                DrawLine (new Point (startX + i * cellSize, startY), new Point (startX + i * cellSize, startY + size), Brushes.Gray);
            }
        }

        // 2. Triangular Mesh on Square (split each quad diagonally)
        private void DrawSquareTriMesh(double startX, double startY, double size, int divisions)
        {
            double cellSize = size / divisions;

            // Draw outer square boundary
            DrawRectangle (startX, startY, size, size, Brushes.Black, 2);

            for (int i = 0; i < divisions; i++)
            {
                for (int j = 0; j < divisions; j++)
                {
                    Point topLeft = new Point (startX + i * cellSize, startY + j * cellSize);
                    Point topRight = new Point (startX + (i + 1) * cellSize, startY + j * cellSize);
                    Point bottomLeft = new Point (startX + i * cellSize, startY + (j + 1) * cellSize);
                    Point bottomRight = new Point (startX + (i + 1) * cellSize, startY + (j + 1) * cellSize);

                    // Draw quad edges (optional, since boundary already drawn)
                    DrawLine (topLeft, topRight, Brushes.Gray);
                    DrawLine (topLeft, bottomLeft, Brushes.Gray);
                    DrawLine (bottomLeft, bottomRight, Brushes.Gray);
                    DrawLine (topRight, bottomRight, Brushes.Gray);

                    // Draw diagonal for two triangles inside quad
                    DrawLine (topLeft, bottomRight, Brushes.DarkBlue, 1);
                }
            }
        }
        

        // 1. Quadrilateral Mesh on Circle (rings x sectors)
        private void DrawCircleQuadraMesh(double centerX, double centerY, double radius, int rings)
        {
            // Draw outer circle boundary
            DrawEllipse (centerX, centerY, radius, Brushes.Black, 2);

            int slices = rings * 2;

            double ringStep = radius / rings;
            double angleStep = 2 * Math.PI / slices;

            // For each ring
            for (int r = 1; r <= rings; r++)
            {
                double innerR = ringStep * (r - 1);
                double outerR = ringStep * r;

                for (int s = 0; s < slices; s++)
                {
                    double angleStart = s * angleStep;
                    double angleEnd = (s + 1) * angleStep;

                    // Convert angles to degrees for easier understanding
                    double degStart = angleStart * 180 / Math.PI;
                    double degEnd = angleEnd * 180 / Math.PI;

                    // Optional: Show in output window
                    Console.WriteLine ($"Ring {r}, Slice {s} => Angle Start: {degStart:F1}°, End: {degEnd:F1}°");

                    // Points for quad
                    Point p1 = new Point (centerX + innerR * Math.Cos (s * angleStep), centerY + innerR * Math.Sin (s * angleStep));
                    Point p2 = new Point (centerX + outerR * Math.Cos (s * angleStep), centerY + outerR * Math.Sin (s * angleStep));
                    Point p3 = new Point (centerX + outerR * Math.Cos ((s + 1) * angleStep), centerY + outerR * Math.Sin ((s + 1) * angleStep));
                    Point p4 = new Point (centerX + innerR * Math.Cos ((s + 1) * angleStep), centerY + innerR * Math.Sin ((s + 1) * angleStep));

                    // Draw quad edges
                    DrawLine (p1, p2, Brushes.Gray);
                    DrawLine (p2, p3, Brushes.Gray);
                    DrawLine (p3, p4, Brushes.Gray);
                    DrawLine (p4, p1, Brushes.Gray);
                }
            }
        }

        // 2. Triangular Mesh on Circle (fan from center)
        private void DrawCircleTriangleMesh(double centerX, double centerY, double radius, int segments)
        {
            Point center = new Point (centerX, centerY);

            // Draw outer circle boundary
            DrawEllipse (centerX, centerY, radius, Brushes.Black, 2);

            double angleStep = 2 * Math.PI / segments;

            Point[] points = new Point[segments];
            for (int i = 0; i < segments; i++)
            {
                points[i] = new Point (centerX + radius * Math.Cos (i * angleStep),
                                      centerY + radius * Math.Sin (i * angleStep));
            }

            for (int i = 0; i < segments; i++)
            {
                // Draw edges from center to circumference
                DrawLine (center, points[i], Brushes.LightBlue);

                // Draw edges between circumference points (closing polygon)
                DrawLine (points[i], points[(i + 1) % segments], Brushes.DarkBlue);
            }
        }


        private void DrawLine(Point p1, Point p2, Brush color, double thickness = 1)
        {
            var line = new Line ()
            {
                X1 = p1.X,
                Y1 = p1.Y,
                X2 = p2.X,
                Y2 = p2.Y,
                Stroke = color,
                StrokeThickness = thickness
            };
            MeshCanvas.Children.Add (line);
        }

        private void DrawRectangle(double x, double y, double width, double height, Brush color, double thickness)
        {
            var rect = new Rectangle ()
            {
                Width = width,
                Height = height,
                Stroke = color,
                StrokeThickness = thickness,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft (rect, x);
            Canvas.SetTop (rect, y);
            MeshCanvas.Children.Add (rect);
        }

        private void DrawEllipse(double centerX, double centerY, double radius, Brush color, double thickness)
        {
            var ellipse = new Ellipse ()
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = color,
                StrokeThickness = thickness,
                Fill = Brushes.Transparent
            };

            Canvas.SetLeft (ellipse, centerX - radius);
            Canvas.SetTop (ellipse, centerY - radius);

            MeshCanvas.Children.Add (ellipse);
        }


        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            // Clear canvas
            MeshCanvas.Children.Clear ();

            if (!int.TryParse (txtBoxzVerticles.Text, out int vertices) || vertices <= 0)
            {
                MessageBox.Show ("Please enter a valid number of vertices.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse (txtBoxTriSegments.Text, out int vertices1) || vertices1 <= 0)
            {
                MessageBox.Show ("Please enter a valid number of vertices.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            btnLoad.IsEnabled = false;
            btnLoad.Content = "Loading...";

            CircleMesh ();
            SquareMesh ();

            btnLoad.IsEnabled = true;
            btnLoad.Content = "Load";
        }
    }
}
