using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nodify.Calculator
{
    internal class ConnectorTypeToShapeConverter : IValueConverter
    {
        ParserContext Context = GetParserContext();
        Dictionary<ConnectorShape, string> ShapeToDrawingGroup = new()
        {
            {
                ConnectorShape.CameraRay,
                """
                <DrawingGroup>
                    <DrawingGroup.ClipGeometry>
                        <RectangleGeometry Rect="0.0,0.0,702.12618,452.37717"/>
                    </DrawingGroup.ClipGeometry>
                    <DrawingGroup Transform="1.0,0.0,0.0,1.0,-148.77382,-274.39">
                        <GeometryDrawing>
                            <GeometryDrawing.Pen>
                                <Pen Brush="{0}" Thickness="35.0" DashCap="Round" EndLineCap="Round" StartLineCap="Round"/>
                            </GeometryDrawing.Pen>
                            <GeometryDrawing.Geometry>
                                <PathGeometry Figures="M 675.78 448.51 L 807 372.74 A 17.6 17.6 0 0 1 833.4 388 V 612 A 17.6 17.6 0 0 1 807 627.26 L 675.78 551.49" FillRule="Nonzero"/>
                            </GeometryDrawing.Geometry>
                        </GeometryDrawing>
                        <GeometryDrawing>
                            <GeometryDrawing.Pen>
                                <Pen Brush="{0}" Thickness="35.0" DashCap="Round" EndLineCap="Round" StartLineCap="Round"/>
                            </GeometryDrawing.Pen>
                            <GeometryDrawing.Geometry>
                                <PathGeometry Figures="m 166.274 662.267 c 0.0659 25.93 21.07 46.934 47 47 h 414.24 c 25.9323 -0.0605 46.9395 -21.0677 47 -47 M 674.81 662.11 V 338.89 c -0.0659 -25.93 -21.07 -46.934 -47 -47 H 213.57 c -25.9323 0.0605 -46.9395 21.0677 -47 47 v 322.22" FillRule="Nonzero"/>
                            </GeometryDrawing.Geometry>
                        </GeometryDrawing>
                    </DrawingGroup>
                </DrawingGroup>
                """
            },
            {
                ConnectorShape.Normals,
                """
                <DrawingGroup>
                    <DrawingGroup.ClipGeometry>
                        <RectangleGeometry Rect="0.0,0.0,1000.0,1000.0"/>
                    </DrawingGroup.ClipGeometry>
                    <DrawingGroup>
                        <GeometryDrawing Brush="{0}">
                            <GeometryDrawing.Pen>
                                <Pen Brush="{0}" Thickness="40" DashCap="Flat"/>
                            </GeometryDrawing.Pen>
                            <GeometryDrawing.Geometry>
                                <PathGeometry Figures="M 37.1797 637.782 L 595.151 79.8105" FillRule="Nonzero"/>
                            </GeometryDrawing.Geometry>
                        </GeometryDrawing>
                        <DrawingGroup>
                            <GeometryDrawing Brush="{0}">
                                <GeometryDrawing.Pen>
                                    <Pen Brush="{0}" Thickness="50" DashCap="Flat">
                                        <Pen.DashStyle>
                                            <DashStyle Dashes="1.0,3.000003802816366"/>
                                        </Pen.DashStyle>
                                    </Pen>
                                </GeometryDrawing.Pen>
                                <GeometryDrawing.Geometry>
                                    <PathGeometry Figures="M 331.111 331.117 L 5.6284 5.63418" FillRule="Nonzero"/>
                                </GeometryDrawing.Geometry>
                            </GeometryDrawing>
                            <DrawingGroup Transform="7.88889,0.0,0.0,7.88889,5.6283963,5.6341848">
                                <GeometryDrawing>
                                    <GeometryDrawing.Geometry>
                                        <PathGeometry Figures="m 1 0 l -3 3 h -2 l 3 -3 l -3 -3 h 2 z"/>
                                    </GeometryDrawing.Geometry>
                                </GeometryDrawing>
                            </DrawingGroup>
                        </DrawingGroup>
                    </DrawingGroup>
                </DrawingGroup>
                """
            },
            {
                ConnectorShape.Color,
                """
                <DrawingGroup>
                    <DrawingGroup.ClipGeometry>
                        <RectangleGeometry Rect="0,0,540.0,540.0"/>
                    </DrawingGroup.ClipGeometry>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <EllipseGeometry RadiusX="255.90427" RadiusY="255.90425" Center="270.0,270.0"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#fffefe33">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="m 511.481 205.295 c 11.9057 44.4326 11.9057 84.9769 0 129.41 L 487.315 349.342 L 270 270 L 490.204 192.1 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#fffb9902">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="m 334.705 28.5185 c 44.4326 11.9057 79.545 32.1778 112.072 64.7048 L 447.4 118.096 L 270 270 L 313.065 40.5081 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#fffabc02">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="m 446.777 93.2233 c 32.5269 32.5269 52.7991 67.6393 64.7048 112.072 L 270 270 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#fffe2712">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 93.2233 93.2233 C 125.75 60.6964 160.863 40.4242 205.295 28.5185 L 231.205 44.5017 L 270 270 L 92.7396 120.057 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#fffd5308">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="m 205.295 28.5185 c 44.4326 -11.9057 84.9769 -11.9057 129.41 0 L 270 270 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#ff8601af">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="m 28.5185 334.705 c -11.9057 -44.4326 -11.9057 -84.9769 0 -129.41 L 56.3113 186.627 L 270 270 L 55.8548 349.405 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#ffa7194b">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 28.5185 205.295 C 40.4242 160.863 60.6964 125.75 93.2233 93.2233 L 270 270 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#ff0247fe">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 205.295 511.481 C 160.863 499.576 125.75 479.304 93.2233 446.777 L 95.3078 418.589 L 270 270 L 231.045 499.706 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#ff3d01a4">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 93.2233 446.777 C 60.6964 414.25 40.4242 379.137 28.5185 334.705 L 270 270 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#ff66b032">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="m 446.777 446.777 c -32.5269 32.5269 -67.6393 52.7991 -112.072 64.7048 L 310.453 496.388 L 270 270 L 446.046 421.157 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#ff0391ce">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="m 334.705 511.481 c -44.4326 11.9057 -84.9769 11.9057 -129.41 0 L 270 270 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="#ffd0ea2b">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 511.481 334.705 C 499.576 379.137 479.304 414.25 446.777 446.777 L 270 270 L 511.481 334.705 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <EllipseGeometry RadiusX="153.79581" RadiusY="153.79581" Center="270.0,270.0"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                </DrawingGroup>
                """
            },
            {
                ConnectorShape.Marcher,
                """
                <DrawingGroup>
                    <DrawingGroup.ClipGeometry>
                        <RectangleGeometry Rect="0.0,0.0,85.284157,77.640099"/>
                    </DrawingGroup.ClipGeometry>
                    <DrawingGroup Transform="1.0,0.0,0.0,1.0,-35.912984,-11.200822">
                        <GeometryDrawing Brush="#00919191">
                            <GeometryDrawing.Pen>
                                <Pen Brush="{0}" Thickness="4" DashCap="Round" EndLineCap="Round" StartLineCap="Round">
                                    <Pen.DashStyle>
                                        <DashStyle Dashes="1.0,3.00000651644098"/>
                                    </Pen.DashStyle>
                                </Pen>
                            </GeometryDrawing.Pen>
                            <GeometryDrawing.Geometry>
                                <EllipseGeometry RadiusX="38.085796" RadiusY="38.085796" Center="78.551949,49.98761"/>
                            </GeometryDrawing.Geometry>
                        </GeometryDrawing>
                        <DrawingGroup>
                            <GeometryDrawing Brush="#00919191">
                                <GeometryDrawing.Pen>
                                    <Pen Brush="{0}" Thickness="3" DashCap="Round" EndLineCap="Round" StartLineCap="Round"/>
                                </GeometryDrawing.Pen>
                                <GeometryDrawing.Geometry>
                                    <PathGeometry Figures="M 36.1638 48.9561 H 78.556 H 120.946" FillRule="Nonzero"/>
                                </GeometryDrawing.Geometry>
                            </GeometryDrawing>
                            <DrawingGroup Transform="0.1327250541666667,0.0,0.0,0.1327250541666667,78.556035,48.956118">
                                <GeometryDrawing>
                                    <GeometryDrawing.Geometry>
                                        <PathGeometry Figures="M -3 3 L 3 -3 M 3 3 L -3 -3" FillRule="Nonzero"/>
                                    </GeometryDrawing.Geometry>
                                </GeometryDrawing>
                            </DrawingGroup>
                        </DrawingGroup>
                    </DrawingGroup>
                </DrawingGroup>
                """
            },
            {
                ConnectorShape.SDF,
                """
                <DrawingGroup>
                    <DrawingGroup.ClipGeometry>
                        <RectangleGeometry Rect="0.0,0.0,512.0,512.0"/>
                    </DrawingGroup.ClipGeometry>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 147.669 208.085 c -0.725 -5.867 -6.123 -10.091 -11.861 -9.323 c -7.531 0.917 -14.805 1.899 -21.781 2.965 c -5.824 0.875 -9.835 6.315 -8.939 12.139 c 0.811 5.291 5.355 9.067 10.517 9.067 c 0.533 0 1.088 -0.043 1.643 -0.128 c 6.784 -1.045 13.824 -1.984 21.099 -2.859 C 144.213 219.243 148.373 213.931 147.669 208.085 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 462.464 215.936 c -6.571 -2.112 -13.803 -4.117 -21.611 -5.995 c -5.995 -1.387 -11.52 2.176 -12.885 7.893 c -1.387 5.739 2.133 11.477 7.872 12.864 c 7.253 1.749 13.952 3.605 20.053 5.568 c 1.067 0.341 2.176 0.512 3.264 0.512 c 4.501 0 8.704 -2.88 10.176 -7.424 C 471.147 223.765 468.075 217.749 462.464 215.936 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 397.461 201.621 c -7.04 -1.045 -14.272 -2.027 -21.76 -2.923 c -5.717 -0.576 -11.157 3.477 -11.861 9.323 s 3.477 11.157 9.323 11.861 c 7.275 0.875 14.315 1.813 21.12 2.837 c 0.533 0.064 1.088 0.107 1.6 0.107 c 5.184 0 9.749 -3.797 10.539 -9.088 C 407.296 207.915 403.264 202.496 397.461 201.621 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 266.645 192.043 L 256 192 l -11.179 0.043 c -5.888 0.043 -10.624 4.864 -10.603 10.752 c 0.043 5.888 4.971 10.496 10.752 10.581 L 256 213.333 l 10.432 0.043 c 0.043 0 0.085 0 0.107 0 c 5.867 0 10.603 -4.693 10.667 -10.581 C 277.269 196.928 272.491 192.085 266.645 192.043 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 83.499 217.899 c -1.408 -5.717 -7.211 -9.237 -12.885 -7.829 c -7.787 1.899 -14.997 3.925 -21.568 6.037 c -5.589 1.813 -8.683 7.808 -6.869 13.419 c 1.472 4.523 5.653 7.403 10.155 7.403 c 1.088 0 2.197 -0.171 3.285 -0.533 c 6.101 -1.963 12.8 -3.84 20.053 -5.611 C 81.387 229.397 84.885 223.637 83.499 217.899 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 512 255.979 c 0 -0.064 -0.021 -0.107 -0.021 -0.149 C 511.893 114.731 397.099 0 256 0 C 114.837 0 0 114.837 0 256 s 114.837 256 256 256 c 141.12 0 255.936 -114.773 256 -255.893 C 511.979 256.064 512 256.021 512 255.979 z M 22.187 239.083 c 8.021 -111.851 94.656 -202.155 204.907 -215.787 c -19.861 35.989 -29.483 107.648 -33.173 170.475 c -4.885 0.277 -9.792 0.533 -14.549 0.875 c -5.888 0.405 -10.325 5.525 -9.899 11.392 c 0.405 5.632 5.077 9.92 10.624 9.92 c 0.277 0 0.533 -0.021 0.747 -0.043 c 3.947 -0.277 8 -0.491 12.032 -0.725 C 192.277 229.845 192 243.733 192 256 c 0 12.288 0.299 26.197 0.875 40.853 C 81.472 290.496 21.333 268.331 21.333 256 c 0 -0.619 0.555 -1.643 1.493 -2.731 C 26.368 249.045 25.941 242.965 22.187 239.083 z M 23.296 284.907 c 36.011 19.883 107.755 29.504 170.624 33.173 c 3.669 62.848 13.291 134.592 33.173 170.624 C 120.811 475.563 36.437 391.189 23.296 284.907 z M 272.875 489.813 c -3.883 -3.733 -9.941 -4.139 -14.144 -0.619 c -1.088 0.917 -2.112 1.472 -2.731 1.472 c -12.331 0 -34.496 -60.16 -40.853 -171.541 C 229.803 319.701 243.691 320 256 320 c 12.288 0 26.155 -0.299 40.811 -0.875 c -0.235 3.84 -0.427 7.744 -0.683 11.52 c -0.384 5.888 4.053 10.965 9.92 11.371 c 0.256 0.021 0.491 0.021 0.747 0.021 c 5.547 0 10.219 -4.309 10.624 -9.984 c 0.32 -4.565 0.555 -9.301 0.832 -13.973 c 62.827 -3.691 134.464 -13.312 170.453 -33.173 C 475.051 395.157 384.747 481.792 272.875 489.813 z M 256 298.667 c -14.656 0 -28.48 -0.341 -41.835 -0.832 c -0.512 -13.355 -0.832 -27.179 -0.832 -41.835 c 0 -151.829 28.181 -234.667 42.667 -234.667 c 0.619 0 1.643 0.555 2.752 1.472 c 1.984 1.664 4.437 2.496 6.848 2.496 c 2.667 0 5.227 -1.173 7.253 -3.115 c 115.904 8.299 208.619 100.992 216.939 216.917 c -3.733 3.883 -4.16 9.963 -0.619 14.144 c 0.939 1.109 1.493 2.133 1.493 2.752 C 490.667 270.485 407.829 298.667 256 298.667 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 319.936 244.779 c -0.064 -5.888 -5.824 -10.923 -10.795 -10.539 c -5.888 0.064 -10.603 4.907 -10.539 10.795 L 298.667 256 l -0.043 10.496 c -0.043 5.888 4.715 10.709 10.603 10.731 c 0.021 0 0.043 0 0.064 0 c 5.845 0 10.624 -4.736 10.667 -10.603 L 320 256 L 319.936 244.779 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 313.259 135.787 c -0.896 -7.509 -1.899 -14.763 -2.965 -21.76 c -0.875 -5.824 -6.571 -9.685 -12.117 -8.96 c -5.845 0.875 -9.835 6.315 -8.96 12.139 c 1.024 6.784 1.984 13.845 2.88 21.12 c 0.64 5.419 5.248 9.387 10.56 9.387 c 0.427 0 0.875 -0.021 1.301 -0.043 C 309.803 146.944 313.963 141.632 313.259 135.787 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 304.043 363.819 c -5.653 -0.619 -11.179 3.456 -11.883 9.323 c -0.896 7.275 -1.835 14.315 -2.88 21.099 c -0.896 5.824 3.115 11.264 8.917 12.16 c 0.533 0.085 1.109 0.128 1.643 0.128 c 5.163 0 9.707 -3.776 10.539 -9.045 c 1.088 -7.019 2.069 -14.272 2.965 -21.781 C 314.048 369.856 309.867 364.544 304.043 363.819 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 294.165 427.968 c -5.824 -1.408 -11.52 2.155 -12.885 7.872 c -1.749 7.253 -3.605 13.995 -5.568 20.096 c -1.792 5.589 1.301 11.605 6.912 13.397 c 1.067 0.341 2.176 0.491 3.243 0.491 c 4.523 0 8.725 -2.88 10.197 -7.403 c 2.069 -6.549 4.096 -13.76 5.973 -21.568 C 303.445 435.115 299.925 429.355 294.165 427.968 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 301.931 70.656 c -1.92 -7.808 -3.904 -15.04 -6.037 -21.589 c -1.813 -5.589 -7.808 -8.661 -13.419 -6.869 c -5.589 1.792 -8.683 7.808 -6.869 13.419 c 1.941 6.08 3.84 12.779 5.589 20.053 c 1.173 4.885 5.547 8.149 10.368 8.149 c 0.832 0 1.664 -0.085 2.517 -0.277 C 299.797 82.155 303.296 76.395 301.931 70.656 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <GeometryDrawing Brush="{0}">
                        <GeometryDrawing.Geometry>
                            <PathGeometry Figures="M 332.096 194.624 c -4.523 -0.341 -9.216 -0.576 -13.888 -0.832 c -0.277 -4.843 -0.533 -9.728 -0.853 -14.464 c -0.405 -5.888 -6.123 -10.432 -11.392 -9.877 c -5.909 0.405 -10.347 5.525 -9.899 11.392 c 0.512 6.976 0.939 14.101 1.301 21.333 c 0.149 2.965 1.515 5.547 3.563 7.403 c 1.792 2.816 4.736 4.885 8.32 5.035 c 7.253 0.363 14.379 0.789 21.333 1.28 c 0.277 0.021 0.512 0.021 0.789 0.021 c 5.525 0 10.219 -4.288 10.603 -9.877 C 342.379 200.149 337.963 195.051 332.096 194.624 z" FillRule="Nonzero"/>
                        </GeometryDrawing.Geometry>
                    </GeometryDrawing>
                    <DrawingGroup Transform="0.966903,-0.255145,0.671941,0.740604,0.0,0.0">
                        <GeometryDrawing Brush="#fe000000">
                            <GeometryDrawing.Pen>
                                <Pen Brush="#00000000" Thickness="4.50996" DashCap="Round" EndLineCap="Round" StartLineCap="Round">
                                    <Pen.DashStyle>
                                        <DashStyle Dashes="1.0,3.0000044346291315"/>
                                    </Pen.DashStyle>
                                </Pen>
                            </GeometryDrawing.Pen>
                            <GeometryDrawing.Geometry>
                                <EllipseGeometry RadiusX="23.557495" RadiusY="38.89756" Center="249.03549,226.12268"/>
                            </GeometryDrawing.Geometry>
                        </GeometryDrawing>
                    </DrawingGroup>
                </DrawingGroup>
                """
            }
        };
        private static ParserContext GetParserContext()
        {
            ParserContext context = new();
            context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            return context;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is ConnectorType connector)
            {
                if (connector.Shape == ConnectorShape.None)
                {
                    return new ControlTemplate(typeof(Control));
                }

                string drawingGroup = string.Format(ShapeToDrawingGroup[connector.Shape], connector.Color.ToString());
                return XamlReader.Parse($$"""

                    <ControlTemplate TargetType="Control">
                        <Image Width="20" Height="20">
                            <Image.Source>
                                <DrawingImage>
                                    <DrawingImage.Drawing>
                                        {{drawingGroup}}
                                    </DrawingImage.Drawing>
                                </DrawingImage>
                            </Image.Source>
                        </Image>
                    </ControlTemplate>
                    """, Context);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
