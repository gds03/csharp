//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace CustomComponents.WinForms.UIControls.Infragistics
//{
//    public partial class WaitForm : FormWithNoCloseButton
//    {
//        public enum Speed
//        {
//            Below_Low = 1,
//            Low = 2,
//            Normal = 3,
//            High = 4,
//            Above_High = 5
//        }

//        public enum DialogType
//        {
//            Thread_Owns_Window = 0,
//            Thread_DontOwn_Window = 1
//        }

//        const int LOW_SPEED = 50;




//        Speed m_speed;



       

//        public WaitForm()
//        {
//            InitializeComponent();

//            MaximizeBox = MinimizeBox = false;
//            m_progress.AnimationSpeed = 50;
//        }






//        #region Public API
        

//        /// <summary>
//        ///     When DialogType Owns window, you must get sure that the job that you want to execute is running in a background thread, otherwise you will block your application.
//        ///     When DialogType dont own window, the thread that calls this method returns immediatly because isn't the owner.
//        /// </summary>
//        public WaitForm StartAndOpen(DialogType t)
//        {
//            m_progress.ResetAnimation();
//            m_progress.Start();
//            CenterToParent();

//            if ( t == DialogType.Thread_Owns_Window )
//            {
//                ShowDialog();
//            }

//            else
//            {
//                Visible = true;
//                Show();
//            }
            
//            return this; 
//        }

//        /// <summary>
//        ///     Stop progressbar and close window.
//        /// </summary>
//        public WaitForm StopAndClose()
//        {
//            m_progress.Stop();
//            Visible = false;
//            return this;
//        }




//        [
//            Category("Options"),
//            Description("Gets or sets the speed for the progressbar"),
//            DefaultValue(Speed.Normal)
//        ]
//        public Speed AnimationSpeed
//        {
//            get { return m_speed; }
//            set
//            {
//                SetSpeed(value);
//            }
//        }


//        [
//            Category("Options"),
//            Description("Gets or sets the style of the progressbar"),
//            DefaultValue(Speed.Normal)
//        ]
//        public ActivityIndicatorViewStyle ViewStyle
//        {
//            get { return m_progress.ViewStyle; }
//            set { m_progress.ViewStyle = value; }
//        }




//        #endregion






//        #region Internal 
        



//        /// <summary>
//        ///     Update m_speed variable and update AnimationSpeed property of Progressbar
//        /// </summary>
//        /// <param name="s"></param>
//        void SetSpeed(Speed s)
//        {
//            m_speed = s;
//            int calc = LOW_SPEED / (int) s;
//            m_progress.AnimationSpeed = calc;
//        }



//        #endregion
//    }
//}
