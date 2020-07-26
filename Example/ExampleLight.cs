using AydenIO.Lifx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AydenIO.Examples.Lifx {
    class ExampleLight : LifxVirtualLight {
        public override string VendorName => "AydenIO";

        public override string ProductName => nameof(ExampleLight);

        public override bool SupportsColor => true;

        public override ushort MinKelvin => UInt16.MinValue;

        public override ushort MaxKelvin => UInt16.MaxValue;

        public ExampleLight(LifxNetwork lifx, MacAddress macAddress) : base(lifx, macAddress) {

        }

        // State
        private class ExampleLightState : ILifxLightState {
            public bool PoweredOn { get; set; }

            public ushort Hue { get; set; }

            public ushort Saturation { get; set; }

            public ushort Brightness { get; set; }

            public ushort Kelvin { get; set; }

            public string Label { get; set; }

            public ILifxHsbkColor ToHsbk() => this;

            public void FromHsbk(ILifxHsbkColor color) {
                if (color != this) {
                    this.Hue = color.Hue;
                    this.Saturation = color.Saturation;
                    this.Brightness = color.Brightness;
                    this.Kelvin = color.Kelvin;
                }
            }
        }

        private readonly ExampleLightState state = new ExampleLightState();

        public override Task<ILifxLightState> GetState(int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult((ILifxLightState)this.state);

        // Power
        public event EventHandler PowerChanged;

        protected bool PoweredOn {
            get => this.state.PoweredOn;
            set {
                if (value != this.state.PoweredOn) {
                    this.state.PoweredOn = value;

                    this.OnPowerChanged();
                }
            }
        }

        protected virtual void OnPowerChanged() {
            this.PowerChanged?.Invoke(this, new EventArgs());
        }

        public override Task SetPower(bool power, TimeSpan duration, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.PoweredOn = power;

            return Task.CompletedTask;
        }

        public override Task<bool> GetPower(int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.PoweredOn);

        // Location
        public event EventHandler LocationChanged;

        private ILifxLocation location;

        protected ILifxLocation Location {
            get => this.location;
            set {
                if (this.location is null || value.UpdatedAt > this.location?.UpdatedAt) {
                    this.location = value;

                    this.OnLocationChanged();
                }
            }
        }

        protected virtual void OnLocationChanged() {
            this.LocationChanged?.Invoke(this, new EventArgs());
        }

        public override Task SetLocation(ILifxLocation location, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Location = location;

            return Task.CompletedTask;
        }

        public override Task<ILifxLocation> GetLocation(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.Location);

        // Group
        public event EventHandler GroupChanged;

        private ILifxGroup group;

        protected ILifxGroup Group {
            get => this.group;
            set {
                if (this.group is null || value.UpdatedAt > this.group?.UpdatedAt) {
                    this.group = value;

                    this.OnGroupChanged();
                }
            }
        }

        protected virtual void OnGroupChanged() {
            this.GroupChanged?.Invoke(this, new EventArgs());
        }

        public override Task SetGroup(ILifxGroup group, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Group = group;

            return Task.CompletedTask;
        }

        public override Task<ILifxGroup> GetGroup(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.Group);

        // Label
        public event EventHandler LabelChanged;

        protected string Label {
            get => this.state.Label;
            set {
                if (value != this.state.Label) {
                    this.state.Label = value;

                    this.OnLabelChanged();
                }
            }
        }

        protected virtual void OnLabelChanged() {
            this.LabelChanged?.Invoke(this, new EventArgs());
        }

        public override Task SetLabel(string label, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Label = label;

            return Task.CompletedTask;
        }

        public override Task<string> GetLabel(bool forceRefresh = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => Task.FromResult(this.Label);

        // Color
        public event EventHandler ColorChanged;

        protected ILifxColor Color {
            get => this.state;
            set {
                ILifxHsbkColor color = value.ToHsbk();

                if (!LifxHsbkColorComparer.Instance.Equals(color, this.state)) {
                    this.state.FromHsbk(color);

                    this.OnColorChanged();
                }
            }
        }

        protected virtual void OnColorChanged() {
            this.ColorChanged?.Invoke(this, new EventArgs());
        }

        public override Task SetColor(ILifxColor color, TimeSpan duration = default, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) {
            this.Color = color;

            return Task.CompletedTask;
        }

        // Waveform
        public override Task SetWaveformOptional(bool transient, ILifxColor color, TimeSpan period, float cycles, short skewRatio, LifxWaveform waveform, bool setHue, bool setSaturation, bool setBrightness, bool setKelvin, bool rapid = false, int? timeoutMs = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
