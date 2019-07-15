using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppBongBan.Services
{
    public class SingleTask<T>
    {
        private CancellationTokenSource m_cts = null;
        private Task<T> m_task = null;
        public CancellationTokenSource Token
        {
            get { return m_cts; }
        }

        public Task<T> CurrentTask
        {
            get { return m_task; }
        }

        public void RunAsync(Func<CancellationTokenSource, Task<T>> t)
        {
            // Neu task dang chay thi cancel
            if (this.m_task != null && !this.m_task.IsCompleted && !this.m_task.IsCanceled)
                if (m_cts != null)
                    m_cts.Cancel();
            m_cts = new CancellationTokenSource();
            this.m_task = t.Invoke(m_cts);
        }
    }
}
