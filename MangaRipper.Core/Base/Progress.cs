using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MangaRipper.Core
{
    public class Progress<T>
    {
        private SynchronizationContext _context;
        private Action<T> _action;

        public Progress(Action<T> action)
        {
            _context = SynchronizationContext.Current ?? new SynchronizationContext();
            _action = action;
        }

        public void ReportProgress(T value)
        {
            _context.Post(delegate
            {
                _action.Invoke(value);
            }, null);
        }
    }
}
