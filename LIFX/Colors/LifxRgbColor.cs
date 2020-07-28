// Copyright (c) Ayden Hull 2020. All rights reserved.
// See LICENSE for more information.

using System;
using System.Linq;

namespace AydenIO.Lifx {
    /// <summary>
    /// Represents an RGB color.
    /// </summary>
    public class LifxRgbColor : ILifxColor {
        /// <summary>Gets or sets the red value for the color.</summary>
        public byte Red { get; set; }

        /// <summary>Gets or sets the green value for the color.</summary>
        public byte Green { get; set; }

        /// <summary>Gets or sets the blue value for the color.</summary>
        public byte Blue { get; set; }

        /// <summary>Gets or sets the kelvin value for the color.</summary>
        public ushort Kelvin { get; set; }

        /// <inheritdoc />
        public void FromHsbk(ILifxHsbkColor hsbk) {
            if (hsbk == null) {
                return;
            }

            // Scale values
            double h = (double)hsbk.Hue / UInt16.MaxValue;
            double s = (double)hsbk.Saturation / UInt16.MaxValue;
            double v = (double)hsbk.Brightness / UInt16.MaxValue;

            // https://en.wikipedia.org/wiki/HSL_and_HSV#HSV_to_RGB
            double c = v * s;
            double hHextant = h / 6.0d;
            double x = c * (1.0d - Math.Abs((hHextant % 2) - 1.0d));

            double r;
            double g;
            double b;

            if (hHextant <= 1.0d) {
                r = c;
                g = x;
                b = 0.0d;
            } else if (hHextant <= 2.0d) {
                r = x;
                g = c;
                b = 0.0d;
            } else if (hHextant <= 3.0d) {
                r = 0.0d;
                g = c;
                b = x;
            } else if (hHextant <= 4.0d) {
                r = 0.0d;
                g = x;
                b = c;
            } else if (hHextant <= 5.0d) {
                r = x;
                g = 0.0d;
                b = c;
            } else {
                r = c;
                g = 0.0d;
                b = x;
            }

            double m = v - c;

            this.Red = Utilities.MultiplyRoundClampUInt8(r + m);
            this.Green = Utilities.MultiplyRoundClampUInt8(g + m);
            this.Blue = Utilities.MultiplyRoundClampUInt8(b + m);
            this.Kelvin = hsbk.Kelvin;
        }

        /// <inheritdoc />
        public ILifxHsbkColor ToHsbk() {
            double colorMax = new[] { this.Red, this.Green, this.Blue }.Max();
            double colorMin = new[] { this.Red, this.Green, this.Blue }.Min();
            double colorDiff = colorMax - colorMin;

            double outHue = 0.0d;
            double outBrightness = colorMax / Byte.MaxValue;
            double outSaturation = 0.0d;

            if (colorMax != 0) {
                outSaturation = colorDiff / colorMax;
            }

            if (outSaturation != 0) {
                double colorRatioR = (colorMax - this.Red) / colorDiff;
                double colorRatioG = (colorMax - this.Green) / colorDiff;
                double colorRatioB = (colorMax - this.Blue) / colorDiff;

                if (colorMax == this.Red) {
                    outHue = colorRatioB - colorRatioG;
                } else if (colorMax == this.Green) {
                    outHue = 2.0d + colorRatioR - colorRatioB;
                } else {
                    outHue = 4.0d + colorRatioG - colorRatioR;
                }

                outHue /= 6.0d;

                if (outHue < 0.0d) {
                    outHue += 1.0d;
                }
            }

            // Convert to HSBK
            return new LifxHsbkColor() {
                Hue = Utilities.MultiplyRoundClampUInt16(outHue),
                Saturation = Utilities.MultiplyRoundClampUInt16(outSaturation),
                Brightness = Utilities.MultiplyRoundClampUInt16(outBrightness),
                Kelvin = this.Kelvin,
            };
        }
    }
}
