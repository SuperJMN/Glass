using System.Windows;

namespace Glass.Design.Designer
{
	public class DesignOptions
	{
		#region CanRotate

		/// <summary>
		/// CanRotate Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty CanRotateProperty =
			DependencyProperty.RegisterAttached("CanRotate", typeof (bool), typeof (DesignOptions),
			                                    new FrameworkPropertyMetadata(false,
			                                                                  FrameworkPropertyMetadataOptions.None));

		/// <summary>
		/// Gets the CanRotate property. This dependency property 
		/// indicates ....
		/// </summary>
		public static bool GetCanRotate(DependencyObject d)
		{
			return (bool) d.GetValue(CanRotateProperty);
		}

		/// <summary>
		/// Sets the CanRotate property. This dependency property 
		/// indicates ....
		/// </summary>
		public static void SetCanRotate(DependencyObject d, bool value)
		{
			d.SetValue(CanRotateProperty, value);
		}

		#endregion

		
	}
}