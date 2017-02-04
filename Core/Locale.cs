
namespace CSim.Core {
	using System;
	using System.Globalization;
	using System.Threading;

	/// <summary>A simplification for L18n, following ISO 3166.</summary>
	public static class Locale {
		/// <summary>The ISO ES-ES locale.</summary>
		public const string EsLocale = "es-ES";
		/// <summary>The ISO EN-US locale.</summary>
		public const string UsLocale = "en-US";

		/// <summary>
		/// Sets the locale, given a string.
		/// An empty locale, or just containing &lt;, sets the system locale.
		/// </summary>
		/// <param name="locale">The locale, as a string such as "ES-ES".</param>
		public static void SetLocale(string locale)
		{
			CultureInfo cultureInfo;

			locale = locale.Trim();
			if ( locale.Length == 0
			  || locale[ 0 ] == '<' )
			{
				cultureInfo = SystemLocale;
			} else {
				cultureInfo = new CultureInfo( locale );
			}

			try {
				Thread.CurrentThread.CurrentCulture = cultureInfo;
				Thread.CurrentThread.CurrentUICulture = cultureInfo;
			}
			catch(Exception) {
				CurrentLocale = SystemLocale;
			}

			return;
		}

		/// <summary>
		/// Sets the locale from its description.
		/// </summary>
		/// <param name="strLocale">The description for the locale.</param>
		public static void SetLocaleFromDescription(string strLocale)
		{
			strLocale = strLocale.Trim();

            if ( strLocale.Length > 0
                 && strLocale[ 0 ] != '<' )
            {
                string[] strLocales = strLocale.Split( ':' );

                if ( strLocales.Length > 1 ) {
                    strLocale = strLocales[ 1 ];
                }
            }
            else {
                strLocale = CurrentLocale.TwoLetterISOLanguageName;
            }

			SetLocale( strLocale );
		}

		/// <summary>
		/// Gets the current locale code.
		/// </summary>
		/// <returns>The current locale code.</returns>
		public static string GetCurrentLocaleCode()
		{
			return GetLocaleCode( CurrentLocale );
		}

		/// <summary>
		/// Gets the locale code from <see cref="CultureInfo"/>.
		/// </summary>
		/// <returns>The locale code, as a string.</returns>
		/// <param name="locale">A <see cref="CultureInfo"/>.</param>
		public static string GetLocaleCode(CultureInfo locale)
		{
			return locale.ToString();
		}

		/// <summary>
		/// Locale to description.
		/// </summary>
		/// <returns>The description of the locale.</returns>
		/// <param name="locale">A <see cref="CultureInfo"/>.</param>
		public static string LocaleToDescription(CultureInfo locale)
		{
			return locale.NativeName + ": " + locale;
		}

		/// <summary>
		/// Gets the description of the current locale.
		/// </summary>
		/// <returns>The description, as a string.</returns>
		public static string CurrentLocaleToDescription()
		{
			return LocaleToDescription( CurrentLocale );
		}

		/// <summary>
		/// Gets or sets the current locale to the currnet thread.
		/// </summary>
		/// <value>The current locale.</value>
		public static CultureInfo CurrentLocale {
			get {
				return Thread.CurrentThread.CurrentCulture;
			}
			set {
				Thread.CurrentThread.CurrentCulture = value;
				Thread.CurrentThread.CurrentUICulture = value;
			}
		}

		/// <summary>
		/// Gets the system locale.
		/// </summary>
		/// <value>The system locale, as a <see cref="CultureInfo"/>.</value>
		public static CultureInfo SystemLocale {
			get {
				return CultureInfo.InstalledUICulture;
			}
		}
	}
}

