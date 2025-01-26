using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using System.Windows.Input;

namespace VoicemailApplication
{
    /// <summary>
    /// The Keypad button
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class KeypadButton : INotifyPropertyChanged
    {
        /// <summary>
        /// The is selected
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The value
        /// </summary>
        private string value;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeypadButton"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public KeypadButton(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set 
            { 
                isSelected = value;
                RaisedPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets the select button.
        /// </summary>
        /// <value>
        /// The select button.
        /// </value>
        public ICommand SelectButton => new RelayCommand(this.ButtonSelected);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value => this.value;

        /// <summary>
        /// Buttons the selected.
        /// </summary>
        private void ButtonSelected()
        {
             this.IsSelected = true;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisedPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
