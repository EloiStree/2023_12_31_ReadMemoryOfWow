partial class Program
{
    public class TimerLoop
    {
        private System.Threading.Timer m_timer;
        public Action m_whatToDo;
        public bool m_isActive;
        public TimerLoop(double intervalInSeconds, Action whatToDo, bool isActive)
        {
            m_isActive = isActive;
            m_whatToDo = whatToDo;
            m_timer = new System.Threading.Timer(DoAction, null, TimeSpan.Zero, TimeSpan.FromSeconds(intervalInSeconds));
        }

        public void SetAsActive(bool isActive) => m_isActive = isActive;

        private void DoAction(object state)
        {
            if (m_isActive)
            {
                if (m_whatToDo != null)
                    m_whatToDo.Invoke();
                
            }
        }

        public void Stop()
        {
            // Stop the timer when you're done
            m_timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        
    }
}
 
