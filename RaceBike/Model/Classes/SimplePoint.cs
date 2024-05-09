using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class SimplePoint : INotifyPropertyChanged
    {
        #region Properties
        private int _x;
        public int X
        {
            get { return _x; }
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }
        private int _y;
        public int Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Constructor
        public SimplePoint(int x = 0, int y = 0)
        {
            // if (x < 0) throw new ArgumentException($"Coordinate {nameof(x)} cannot take value {x}.");
            // if (y < 0) throw new ArgumentException($"Coordinate {nameof(y)} cannot take value {y}.");
            
            // I've just realised that the environment around the bike can contain negative coordinates

            X = x;
            Y = y;
        }
        #endregion

        #region Public methods
        public override string ToString()
        {
            return string.Format($"({X},{Y})");
        }

        public static SimplePoint Parse(string s)
        {
            int x, y;
            int i = 0;

            while (char.IsWhiteSpace(s[i]) && i < s.Length) i++;

            if (s[i] != '(') throw new FormatException("Character '(' was not found");
            i++;

            var builder = new StringBuilder();

            while (s[i] != ',' && i < s.Length)
            {
                builder.Append(s[i]);
                i++;
            }

            try
            {
                x = Convert.ToInt32(builder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            builder.Clear();

            while ((s[i] == ',' || char.IsWhiteSpace(s[i])) && i < s.Length) i++;

            while (s[i] != ')' && i < s.Length)
            {
                builder.Append(s[i]);
                i++;
            }

            try
            {
                y = Convert.ToInt32(builder.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return new SimplePoint(x, y);
        }
        #endregion

        #region Protected methods
        protected virtual void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged is not null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
