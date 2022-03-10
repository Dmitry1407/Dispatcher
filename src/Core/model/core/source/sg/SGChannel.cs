using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Threading;

using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.core.source.sg
{
    public class SGChannel : Channel
    {
        [SortedCategory("Generator", 2, 10), PropertyOrder(0)]
        [DisplayName("SGType")]
        [Description("Generator Type")]
        public SGType SGType { get; set; }

        [SortedCategory("Generator", 2, 10), PropertyOrder(1)]
        [DisplayName("Step")]
        [Description("Step size")]
        public Double Step { get; set; }

        [SortedCategory("Generator", 2, 10), PropertyOrder(2)]
        [DisplayName("MinValue")]
        [Description("MinValue")]
        public Double MinValue { get; set; }

        [SortedCategory("Generator", 2, 10), PropertyOrder(3)]
        [DisplayName("MaxValue")]
        [Description("MaxValue")]
        public Double MaxValue { get; set; }

        [Browsable(false)]
        public Double Value { get; private set; }

        public SGChannel() : this(0, 0D, 100D) { }

        public SGChannel(Int32 id, Double minValue, Double maxValue)
            : base(id)
        {
            ID = id;
            IsEnable = true;

            Type = ChannelType.SG;
            SGType = SGType.Sawtooth;

            Step = 1D;
            MinValue = minValue;
            MaxValue = maxValue;
            Value = minValue;
        }

        public void Init()
        {
            Value = MinValue;
        }

        public void UpdateValue()
        {
            switch (SGType)
            {
                case SGType.Square:
                    UpdateSquare();
                    break;
                case SGType.Triangle:
                    UpdateTriangle();
                    break;
                case SGType.Sawtooth:
                    UpdateSawtooth();
                    break;
                case SGType.Sine:
                    UpdateSine();
                    break;
                case SGType.Random:
                    UpdateRandom();
                    break;
            }
        }

        private Int32 stepCounter = 0;
        private void UpdateSquare()
        {
            if (stepCounter++ >= Step)
            {
                if (MaxValue - Value < 0.01D) { Value = MinValue; }
                else { Value = MaxValue; }
                stepCounter = 0;
            }
        }

        private Boolean up = true;
        private void UpdateTriangle()
        {
            if ((Value + Step) - MaxValue > 0.01D) { up = false; }
            else if (MinValue - (Value - Step) > 0.01D) { up = true; }
            Value = up ? Value + Step : Value - Step;
        }

        private void UpdateSawtooth()
        {
            if (Value - MaxValue > 0.01D) { Value = MinValue; }
            else { Value += Step; }
        }

        // Angle in radians
        private Double angle = 0D;
        private void UpdateSine()
        {
            if (angle >= Math.PI * 2) { angle = 0D; }
            Value = MinValue + Math.Sin(angle) * (MaxValue - MinValue);
            angle += 0.1D;
        }

        private System.Random random = new System.Random();
        private void UpdateRandom()
        {
            Value = random.Next((Int32)MinValue, (Int32)MaxValue);
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > 0D; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? 1D : 0D; }
        public override void SetIntValue(Int32 value) { Value = (Double)value; }
        public override void SetUIntValue(UInt32 value) { Value = (Double)value; }
        public override void StFloatValue(Single value) { Value = (Double)value; }
        public override void SetDoubleValue(Double value) { Value = value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = Double.Parse(value);
            }
        }

    }
}
