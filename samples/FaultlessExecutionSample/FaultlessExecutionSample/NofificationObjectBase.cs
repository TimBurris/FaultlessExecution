using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultlessExecutionSample
{
    public abstract class NotificationObjectBase : System.ComponentModel.INotifyPropertyChanged
    {
        #region Property Changed Functionality

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String caller = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName: caller));
        }

        protected virtual void OnPropertyChanged(System.Linq.Expressions.Expression<Func<object>> expression)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName: this.ExtractPropertyName(expression)));
        }

        private String ExtractPropertyName(System.Linq.Expressions.Expression<Func<object>> expression)
        {

            if (expression.NodeType != System.Linq.Expressions.ExpressionType.Lambda)
                throw new ArgumentException("Value must be a lamda expression", "expression");

            var body = expression.Body as System.Linq.Expressions.MemberExpression;

            if (body == null)
                throw new ArgumentException("'x' should be a member expression");

            string propertyName = body.Member.Name;
            return propertyName;
        }

        #endregion

    }
}
