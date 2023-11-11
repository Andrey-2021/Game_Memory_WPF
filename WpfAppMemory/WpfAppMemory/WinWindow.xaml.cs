using System.Windows;

namespace WpfAppMemory
{
	/// <summary>
	/// Логика взаимодействия для WinWindow.xaml
	/// </summary>
	public partial class WinWindow : Window
	{
		public WinWindow()
		{
			InitializeComponent();
		}

		private void Button_Click_StartGame(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
    }
}
