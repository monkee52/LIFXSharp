using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AydenIO.Lifx {
    public class LifxRgbColor : ILifxColor {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public ushort Kelvin { get; set; }

        public void FromHsbk(ILifxHsbkColor hsbk) {
            double H = (double)hsbk.Hue / UInt16.MaxValue;
            double S = (double)hsbk.Saturation / UInt16.MaxValue;
            double V = (double)hsbk.Brightness / UInt16.MaxValue;

            double C = V * S;
            double Hd = H / 6.0d;
            double X = C * (1.0d - Math.Abs(Hd % 2 - 1.0d));

            double R;
            double G;
            double B;

            if (Hd <= 1.0d) {
                R = C;
                G = X;
                B = 0.0d;
            } else if (Hd <= 2.0d) {
                R = X;
                G = C;
                B = 0.0d;
            } else if (Hd <= 3.0d) {
                R = 0.0d;
                G = C;
                B = X;
            } else if (Hd <= 4.0d) {
                R = 0.0d;
                G = X;
                B = C;
            } else if (Hd <= 5.0d) {
                R = X;
                G = 0.0d;
                B = C;
            } else {
                R = C;
                G = 0.0d;
                B = X;
            }

            double m = V - C;

            this.Red = Utilities.MultiplyRoundClampUInt8(R + m);
            this.Green = Utilities.MultiplyRoundClampUInt8(G + m);
            this.Blue = Utilities.MultiplyRoundClampUInt8(B + m);
            this.Kelvin = hsbk.Kelvin;
        }

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
                Kelvin = this.Kelvin
            };
        }
    }
}
