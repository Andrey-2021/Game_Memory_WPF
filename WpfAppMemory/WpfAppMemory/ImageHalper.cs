using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace WpfAppMemory
{
	/// <summary>
	/// Загрузка картинок
	/// </summary>
	internal class ImageHalper
	{
		/// <summary>
		/// Загрузить все картинки из папки
		/// </summary>
		/// <returns>Список картинок BitmapImage</returns>
		public static List<BitmapImage> LoadImages()
		{
			List<BitmapImage> list = new List<BitmapImage>();

			string dir = AppDomain.CurrentDomain.BaseDirectory; //каталог программы
			var files = Directory.GetFiles(dir + "\\Images", "*.png"); //картинки из папки

			foreach (var file in files) //перебираем все картинки
			{
				BitmapImage image = new BitmapImage(new Uri(file));
				list.Add(image);
			}
			return list;
		}
	}
}
