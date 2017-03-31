using SQL2Search.Compiler;
using SQL2Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarejoSimples.Tasks;

namespace VarejoSimples.Views.Setup
{
    /// <summary>
    /// Interaction logic for PreparacaoSistema.xaml
    /// </summary>
    public partial class PreparacaoSistema : Window
    {
        public PreparacaoSistema()
        {
            InitializeComponent();
            
            FadeIn(GridExecutando);
            SetupTask task = new SetupTask(this);
            task.Execute(1);
        }

        private void FadeIn(Grid grid)
        {
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = TimeSpan.FromMilliseconds(3000); //

            DoubleAnimation fadeInAnimation = new DoubleAnimation()
            { From = 0.0, To =2, Duration = new Duration(duration) };

          //  DoubleAnimation fadeOutAnimation = new DoubleAnimation()
          //  { From = 2, To = 0.0, Duration = new Duration(duration) };
          //  fadeOutAnimation.BeginTime = TimeSpan.FromSeconds(5);

            Storyboard.SetTargetName(fadeInAnimation, grid.Name);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity", 1));
            storyboard.Children.Add(fadeInAnimation);
            storyboard.Begin(grid);
        }
    }
}
