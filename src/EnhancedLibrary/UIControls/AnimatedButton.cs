using System.ComponentModel;
using Infragistics.Win.UltraActivityIndicator;
using System.Windows.Forms;
using System.Drawing;
using Infragistics.Win.Misc;

namespace EnhancedLibrary.UIControls
{
    public sealed class AnimatedButton : Panel
    {
        private const int SPEED = 20;
        private readonly UltraActivityIndicator m_indicator;
        private readonly UltraButton m_button;


        public AnimatedButton()
        {
            m_button = new UltraButton();
            m_indicator = new UltraActivityIndicator();

            m_button.Text = Text;
            m_button.Size = m_indicator.Size = new Size(Size.Width, Size.Height);

            m_button.Visible = true;
            m_indicator.Visible = false;

            // Add activity to the button
            Controls.Add(m_button);
            Controls.Add(m_indicator);

            Resize += (sender, args) => UpdateControlsSize();
            m_button.Click += (sender, args) => OnClick(args);
            m_indicator.AnimationSpeed = SPEED;
        }

        private void UpdateControlsSize()
        {
            m_button.Size = m_indicator.Size = new Size(Size.Width, Size.Height);
        }


        public AnimatedButton(int speed)
            : this()
        {
            m_indicator.AnimationSpeed = speed;
        }










        /// <summary>
        ///     Starts the progress animation
        /// </summary>
        public void StartAnimation()
        {
            m_button.Visible = false;
            m_indicator.Visible = true;

            m_indicator.Start(true);
        }


        /// <summary>
        ///     Stops the progress animation
        /// </summary>
        public void StopAnimation()
        {
            m_button.Visible = true;
            m_indicator.Visible = false;

            m_indicator.Stop();
        }







        #region Properties

        [Browsable(true)]
        public override string Text
        {
            get { return m_button.Text; }
            set { m_button.Text = value; }
        }


        /// <summary>
        ///     Get or sets the image to be displayed on button control
        /// </summary>
        [Browsable(true)]
        public Image Image
        {
            get { return (Image) m_button.Appearance.Image; }
            set { m_button.Appearance.Image = value; }
        }

        [Browsable(true)]
        public int SpeedAnimation
        {
            get { return m_indicator.AnimationSpeed; }
            set { m_indicator.AnimationSpeed = value; }
        }

        [Browsable(true)]
        public MarqueeAnimationStyle MarqueeAnimationStyle
        {
            get { return m_indicator.MarqueeAnimationStyle; }
            set { m_indicator.MarqueeAnimationStyle = value; }
        }

        [Browsable(true)]
        public ActivityIndicatorViewStyle ViewStyle
        {
            get { return m_indicator.ViewStyle; }
            set { m_indicator.ViewStyle = value; }
        }


        #endregion


    }
}
