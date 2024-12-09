using System;

public class Observable<T>
{
    private T _value;
    
    public class ChangedEventArgs : EventArgs
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }  
    }
    
    public event EventHandler<ChangedEventArgs> OnChangedEvent;
    private ChangedEventArgs _args = new ChangedEventArgs();
    
    public T Value
    {
        get => _value;
        
        set
        {
            if (!value.Equals(_value))
            {
                T oldValue = _value;
                _value = value;
                
                EventHandler<ChangedEventArgs> handler = OnChangedEvent;
                if(handler != null)
                {
                    _args.OldValue = oldValue;
                    _args.NewValue = _value;
                    handler(this, _args);
                }
            }
        }
    }
}
