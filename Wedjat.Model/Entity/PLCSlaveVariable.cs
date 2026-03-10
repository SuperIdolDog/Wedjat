using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wedjat.Model.Entity
{
    /// <summary>
    /// PLC的从站变量
    /// </summary>
    public class PLCSlaveVariable : BaseModel
    {
       
        public int SlaveAddress { get; set; }
        public string DataBlockAddress {  get; set; }
       
        public string VariableName { get; set; }
      
        public string Description { get; set; }
        public string DataType {  get; set; }

        private double _currentValue;

        public int ModbusAddress {  get; set; }

        public string ModbusType {  get; set; }
       
        public bool IsWritable {  get; set; }
        public string DataLength {  get; set; }

        
        public DateTime updateTime {  get; set; }

        
        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                if (Math.Abs(_currentValue - value) > 0.0001) 
                {
                    _currentValue = value;
                    OnPropertyChanged(); 
                }
            }
        }

       
    }
}