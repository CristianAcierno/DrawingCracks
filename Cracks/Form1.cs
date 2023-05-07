using System.Drawing.Imaging;

namespace Cracks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public static Bitmap DetectCracks(Bitmap image, int minThreshold, int maxThreshold, int crackThreshold)
        {
            // Convert the input image to grayscale
            Bitmap grayImage = Grayscale(image);

            // Compute the gradient magnitude and direction images
            Bitmap xGradImage = ComputeGradient(grayImage, true);
            Bitmap yGradImage = ComputeGradient(grayImage, false);
            Bitmap magImage = ComputeMagnitude(xGradImage, yGradImage);
            Bitmap dirImage = ComputeDirection(xGradImage, yGradImage);

            // Perform non-maximum suppression to thin out the edges
            Bitmap suppressedImage = NonMaximumSuppression(magImage, dirImage);

            // Apply hysteresis thresholding to identify strong and weak edges
            int[,] thresholded = Threshold(suppressedImage, minThreshold);
            int[,] edgeMap = Hysteresis(thresholded, 50, 255);

            // Find the contours in the edge map
            List<List<Point>> contours = FindContours(edgeMap, crackThreshold);

            // Draw the contours on the input image
            Bitmap resultImage = new Bitmap(image);
            using (Graphics g = Graphics.FromImage(resultImage))
            {
                foreach (var contour in contours)
                {
                    if (contour.Count >= 3)
                    {
                        Point[] points = contour.ToArray();
                        g.DrawPolygon(Pens.Red, points);
                    }
                }
            }

            return resultImage;
        }
        private static Point FindNextPoint(Point current, int[,] edgeMap, bool[,] visited, int crackThreshold)
        {
            // Search in a 3x3 neighborhood for the next point in the contour
            for (int y = current.Y - 1; y <= current.Y + 1; y++)
            {
                for (int x = current.X - 1; x <= current.X + 1; x++)
                {
                    // If this pixel has already been visited or is outside the image bounds, skip it
                    if (y < 0 || y >= edgeMap.GetLength(0) || x < 0 || x >= edgeMap.GetLength(1) || visited[y, x])
                    {
                        continue;
                    }

                    // If this pixel is an edge pixel and is close enough to the current pixel, return it as the next point
                    if (edgeMap[y, x] != 0 && Distance(current, new Point(x, y)) <= crackThreshold)
                    {
                        return new Point(x, y);
                    }
                }
            }

            // If no unvisited edge pixel was found, return an empty point
            return Point.Empty;
        }
        private static double Distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        private static List<List<Point>> FindContours(int[,] edgeMap, int crackThreshold)
        {
            List<List<Point>> contours = new List<List<Point>>();

            // Create visited map and initialize to false
            bool[,] visited = new bool[edgeMap.GetLength(0), edgeMap.GetLength(1)];

            // Search for cracks in each row
            for (int y = 0; y < edgeMap.GetLength(0); y++)
            {
                for (int x = 0; x < edgeMap.GetLength(1); x++)
                {
                    // If this pixel has already been visited or is not an edge pixel, skip it
                    if (visited[y, x] || edgeMap[y, x] == 0)
                    {
                        continue;
                    }

                    // Create a new contour
                    List<Point> contour = new List<Point>();
                    contour.Add(new Point(x, y));

                    // Initialize the current point to the starting point
                    Point current = new Point(x, y);

                    // Loop until the contour is closed
                    bool isClosed = false;
                    while (!isClosed)
                    {
                        visited[current.Y, current.X] = true;

                        // Search for the next point in the contour
                        Point next = FindNextPoint(current, edgeMap, visited, crackThreshold);
                        if (next == Point.Empty)
                        {
                            // If no next point was found, the contour is closed
                            isClosed = true;
                        }
                        else
                        {
                            // Add the next point to the contour
                            contour.Add(next);

                            // Set the current point to the next point
                            current = next;
                        }
                    }

                    contours.Add(contour);
                }
            }

            return contours;
        }
        private static int[,] Threshold(Bitmap inputImage, int threshold)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;
            int[,] binaryImage = new int[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int value = inputImage.GetPixel(x, y).R;

                    if (value > threshold)
                    {
                        binaryImage[y, x] = 255;
                    }
                    else
                    {
                        binaryImage[y, x] = 0;
                    }
                }
            }

            return binaryImage;
        }
        private static int GetPixelValue(Bitmap image, int x, int y)
        {
            if (x >= 0 && x < image.Width && y >= 0 && y < image.Height)
            {
                return image.GetPixel(x, y).R;
            }
            else
            {
                return 0;
            }
        }
        private static Bitmap NonMaximumSuppression(Bitmap magImage, Bitmap dirImage)
        {
            Bitmap nmsImage = new Bitmap(magImage.Width, magImage.Height);

            for (int y = 0; y < magImage.Height; y++)
            {
                for (int x = 0; x < magImage.Width; x++)
                {
                    double angle = dirImage.GetPixel(x, y).GetHue();

                    int mag1 = 0, mag2 = 0;

                    // Compute the magnitudes of the two neighboring pixels in the gradient direction
                    if (angle >= 337.5 || angle < 22.5 || (angle >= 157.5 && angle < 202.5))
                    {
                        if (x + 1 < magImage.Width)
                        {
                            mag1 = GetGrayValue(magImage.GetPixel(x + 1, y));
                        }
                        if (x - 1 >= 0)
                        {
                            mag2 = GetGrayValue(magImage.GetPixel(x - 1, y));
                        }
                    }
                    else if ((angle >= 22.5 && angle < 67.5) || (angle >= 202.5 && angle < 247.5))
                    {
                        if (x + 1 < magImage.Width && y - 1 >= 0)
                        {
                            mag1 = GetGrayValue(magImage.GetPixel(x + 1, y - 1));
                        }
                        if (x - 1 >= 0 && y + 1 < magImage.Height)
                        {
                            mag2 = GetGrayValue(magImage.GetPixel(x - 1, y + 1));
                        }
                    }
                    else if ((angle >= 67.5 && angle < 112.5) || (angle >= 247.5 && angle < 292.5))
                    {
                        if (y - 1 >= 0)
                        {
                            mag1 = GetGrayValue(magImage.GetPixel(x, y - 1));
                        }
                        if (y + 1 < magImage.Height)
                        {
                            mag2 = GetGrayValue(magImage.GetPixel(x, y + 1));
                        }
                    }
                    else
                    {
                        if (x - 1 >= 0 && y - 1 >= 0)
                        {
                            mag1 = GetGrayValue(magImage.GetPixel(x - 1, y - 1));
                        }
                        if (x + 1 < magImage.Width && y + 1 < magImage.Height)
                        {
                            mag2 = GetGrayValue(magImage.GetPixel(x + 1, y + 1));
                        }
                    }

                    int mag = GetGrayValue(magImage.GetPixel(x, y));

                    // Perform non-maximum suppression
                    if (mag >= mag1 && mag >= mag2)
                    {
                        nmsImage.SetPixel(x, y, Color.FromArgb(mag, mag, mag));
                    }
                    else
                    {
                        nmsImage.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return nmsImage;
        }

        private static Bitmap ComputeMagnitude(Bitmap xGradImage, Bitmap yGradImage)
        {
            Bitmap magImage = new Bitmap(xGradImage.Width, xGradImage.Height);

            for (int y = 0; y < xGradImage.Height; y++)
            {
                for (int x = 0; x < xGradImage.Width; x++)
                {
                    int xGrad = GetGrayValue(xGradImage.GetPixel(x, y));
                    int yGrad = GetGrayValue(yGradImage.GetPixel(x, y));

                    int magnitude = (int)Math.Sqrt(xGrad * xGrad + yGrad * yGrad);

                    magImage.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }

            return magImage;
        }
        private static int GetGrayValue(Color color)
        {
            int grayValue = (int)((color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11));
            return Math.Max(0, Math.Min(grayValue, 255));
        }
        private static Bitmap ComputeGradient(Bitmap inputImage, bool horizontal)
        {
            Bitmap gradImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    int p1, p2, p3, p4, p5, p6, p7, p8, p9;

                    if (y == 0 || y == inputImage.Height - 1 || x == 0 || x == inputImage.Width - 1)
                    {
                        gradImage.SetPixel(x, y, Color.Black);
                        continue;
                    }

                    p1 = GetGrayValue(inputImage.GetPixel(x - 1, y - 1));
                    p2 = GetGrayValue(inputImage.GetPixel(x, y - 1));
                    p3 = GetGrayValue(inputImage.GetPixel(x + 1, y - 1));
                    p4 = GetGrayValue(inputImage.GetPixel(x - 1, y));
                    p5 = GetGrayValue(inputImage.GetPixel(x, y));
                    p6 = GetGrayValue(inputImage.GetPixel(x + 1, y));
                    p7 = GetGrayValue(inputImage.GetPixel(x - 1, y + 1));
                    p8 = GetGrayValue(inputImage.GetPixel(x, y + 1));
                    p9 = GetGrayValue(inputImage.GetPixel(x + 1, y + 1));

                    int gx = (p3 + 2 * p6 + p9) - (p1 + 2 * p4 + p7);
                    int gy = (p1 + 2 * p2 + p3) - (p7 + 2 * p8 + p9);

                    int gradient = (int)Math.Sqrt(gx * gx + gy * gy);
                   // gradient = (int)(255 * gradient / Math.Sqrt(2 * 255 * 255));
                    double maxGradient = Math.Sqrt(2) * 255;
                    gradient = (int)(255 * gradient / maxGradient);
                    if(gradient > 255)
                        gradient= 255;
                    if (horizontal)
                    {
                        gradImage.SetPixel(x, y, Color.FromArgb(gradient, 0, 0));
                    }
                    else
                    {
                        gradImage.SetPixel(x, y, Color.FromArgb(0, gradient, 0));
                    }
                }
            }

            return gradImage;
        }
        private static Bitmap Grayscale(Bitmap inputImage)
        {
            Bitmap grayImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color color = inputImage.GetPixel(x, y);
                    int grayValue = GetGrayValue(color);
                    grayImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            return grayImage;
        }
        private static Bitmap ComputeDirection(Bitmap xGradImage, Bitmap yGradImage)
        {
            Bitmap dirImage = new Bitmap(xGradImage.Width, xGradImage.Height);

            for (int y = 0; y < xGradImage.Height; y++)
            {
                for (int x = 0; x < xGradImage.Width; x++)
                {
                    int xGrad = GetGrayValue(xGradImage.GetPixel(x, y));
                    int yGrad = GetGrayValue(yGradImage.GetPixel(x, y));
                    double direction = Math.Atan2(yGrad, xGrad) * (180 / Math.PI);
                    int dirValue = (int)Math.Round(direction / 45) * 45 % 180;
                    dirImage.SetPixel(x, y, Color.FromArgb(dirValue, dirValue, dirValue));
                }
            }

            return dirImage;
        }
        private static int[,] Hysteresis(int[,] thresholded, int weakPixelValue, int strongPixelValue)
        {
            int width = thresholded.GetLength(1);
            int height = thresholded.GetLength(0);

            int[,] edgeMap = new int[height, width];

            // Visit all pixels in the thresholded image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // If the pixel is a strong edge pixel, set it in the edge map
                    if (thresholded[y, x] == strongPixelValue)
                    {
                        edgeMap[y, x] = strongPixelValue;
                    }
                    // If the pixel is a weak edge pixel, check its 8 neighbors for strong edge pixels
                    else if (thresholded[y, x] == weakPixelValue)
                    {
                        // Check the 8-connected neighborhood
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            for (int i = x - 1; i <= x + 1; i++)
                            {
                                // Skip the current pixel
                                if (i == x && j == y)
                                {
                                    continue;
                                }

                                // If the neighbor is a strong edge pixel, set the current pixel in the edge map
                                if (j >= 0 && j < height && i >= 0 && i < width && thresholded[j, i] == strongPixelValue)
                                {
                                    edgeMap[y, x] = weakPixelValue;
                                    break;
                                }
                            }

                            if (edgeMap[y, x] == weakPixelValue)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return edgeMap;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                // dlg.Filter = "bmp files (*.bmp)|*.bmp";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Create a new Bitmap object from the picture file on disk,
                    // and assign that to the PictureBox.Image property
                    pbxMain.Image = new Bitmap(dlg.FileName);
                    // Save address of the Picture
                    tbxPicture.Text = dlg.FileName;
                }
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            pbxResult.Image = DetectCracks((Bitmap)pbxMain.Image, 100, 255, 50);
        }
    }
}