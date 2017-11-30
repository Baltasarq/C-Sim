// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
    using System.Globalization;
    using System.Collections.ObjectModel;

    /// <summary>
    /// L18n.
    /// Contains all strings needed in the app
    /// </summary>
	public static class L10n {

        /// Identifies all available strings
		public enum Id {
			///<summary>Action: "reset."</summary>
			ActReset,
			///<summary>Action: "open."</summary>
			ActOpen,
			///<summary>Action: "Save."</summary>
			ActSave,
			///<summary>Action: "to hex."</summary>
			ActInHex,
			///<summary>Action: "to dec."</summary>
			ActInDec,
			///<summary>Action: "zoom in."</summary>
			ActZoomIn,
			///<summary>Action: "zoom out."</summary>
			ActZoomOut,
			///<summary>Action: "show memory."</summary>
			ActShowMemory,
			///<summary>Action: "show diagram."</summary>
			ActShowDiagram,
			///<summary>Action: "help."</summary>
			ActHelp,
			///<summary>Action: "about."</summary>
			ActAbout,
			///<summary>Action: "settings."</summary>
			ActSettings,
			///<summary>Action: "play."</summary>
			ActPlay,
			///<summary>Action: "stop."</summary>
			ActStop,
			///<summary>Status: "ready."</summary>
			StReady,
			///<summary>Status: "reading configuration."</summary>
			StReadingConfig,
			///<summary>Status: "writing configuration."</summary>
			StWritingConfig,
			///<summary>Label "language"</summary>
			LblLanguage,
			///<summary>Label "pointer"</summary>
            LblPointer,
			///<summary>Label "variable"</summary>
            LblVble,
			///<summary>Label "reference"</summary>
            LblRef,
			///<summary>Exception: "while parsing."</summary>
			ExcParsing,
			///<summary>Exception: "memory exhausted."</summary>
            ExcMemoryExhausted,
			///<summary>Exception: "duplicated variable."</summary>
            ExcDuplicatedVble,
			///<summary>Exception: "invalid id."</summary>
            ExcInvalidId,
			///<summary>Exception: "invalid maximum memory."</summary>
            ExcInvalidMaxMemory,
			///<summary>Exception: "invalid memory address."</summary>
            ExcInvalidMemory,
			///<summary>Exception: "type mismatch."</summary>
            ExcTypeMismatch,
			///<summary>Exception: "unknown type."</summary>
            ExcUnknownType,
			///<summary>Exception: "unknown variable."</summary>
            ExcUnknownVble,
			///<summary>Exception: "error at runtime."</summary>
            ExcRuntime,
            ///<summary>Exception: "accessing at address."</summary>
            ErrAccessingAt,
			///<summary>Exception: "reserving memory."</summary>
            ErrReserving,
			///<summary>Exception: "char is not allowed."</summary>
            ErrNotAllowedChar,
			///<summary>Exception: "invalid first char."</summary>
            ErrFirstChar,
			///<summary>Exception: "expected."</summary>
            ErrExpected,
			///<summary>Exception: "expected literal or id."</summary>
            ErrExpectedLiteralOrId,
			///<summary>Exception: "incorrect use of '*'."</summary>
            ErrBadUseStar,
			///<summary>Exception: "unexpected."</summary>
            ErrUnexpected,
			///<summary>Exception: "end of line."</summary>
            ErrEOL,
			///<summary>Exception: "initalizing reference."</summary>
            ErrRefInit,
			///<summary>Exception: "not a pointer."</summary>
            ErrNotAPointer,
			///<summary>Exception: "reference is not set."</summary>
            ErrRefNotSet,
			///<summary>Exception: "trying to set reference more than once."</summary>
            ErrRefDoubleSet,
			///<summary>Exception: "memory is not in heap."</summary>
            ErrMemoryNotInHeap,
			///<summary>Exception: "expected beginning of parameters."</summary>
			ErrExpectedParametersBegin,
			///<summary>Exception: "expected ending of parameters."</summary>
			ErrExpectedParametersEnd,
			///<summary>Exception: "erroneous parameter count."</summary>
			ErrParamCount,
			///<summary>Exception: "invalid parameter type."</summary>
			ErrParamType,
			///<summary>Exception: "function does not exist."</summary>
			ErrFunctionNotFound,
			///<summary>Exception: "expected literal number."</summary>
			ErrExpectedLiteralNumber,
			///<summary>Exception: "cannot derreference void*."</summary>
			ErrDerreferencedVoid,
			///<summary>Exception: "missing arguments."</summary>
			ErrMissingArguments
		};

        /// <summary>
        /// All strings in EN (english)
        /// </summary>
		public static readonly ReadOnlyCollection<string> StringsEN =
			new ReadOnlyCollection<string>( new string[] {
				"Reset",
				"Open",
				"Save",
				"In hexadecimal",
				"In decimal",
				"Zoom In",
				"Zoom out",
				"Show memory",
				"Show diagram",
				"Help",
				"About.",
                "Settings",
				"Run",
				"Stop",
                "Ready",
                "Reading configuration",
                "Writing configuration",
                "Language",
                "Pointer",
                "Variable",
                "Reference",
				"parsing error",
                "memory exhausted",
                "variable already existing with id",
                "invalid id",
                "max memory should be > 16 and %16 == 0",
                "invalid memory address",
                "type mismatch; expected type",
                "unknown type",
                "unknown variable",
                "during run-time",
                "accessing memory at",
                "reserving memory for",
                "char not allowed",
                "first char should be a letter",
                "expected",
                "expected literal or id",
                "incorrect use of derreference (\"*\")",
                "unexpected",
                "end of statement",
                "references must be initialized",
                "not a pointer",
                "reference is not initialized",
                "reference already initialized",
                "memory address incorrect or not in heap",
				"expected begin of parameters with '('",
				"expected end of parameters with ')'",
				"{0}() does not exist with {1} arguments, needs {2}.",
				"param #{0}:'{1}' for {2}() should be of type {3}, instead of {4}",
				"function {0}() was not found",
				"expected number as in: 0, 0.0 or 0x0",
				"tried to derreference void *",
				"not enough arguments in function call"
			}
		);

        /// <summary>
        /// All strings in spanish (ES)
        /// </summary>
		public static readonly ReadOnlyCollection<string> StringsES =
			new ReadOnlyCollection<string>( new string[] {
				"Reiniciar",
				"Abrir",
				"Guardar",
				"En hexadecimal",
				"En decimal",
                "Aumentar",
                "Reducir",
				"Mostrar memoria",
				"Mostrar diagrama",
				"Ayuda",
				"Acerca de...",
                "Preferencias",
				"Ejecutar",
				"Parar",
                "Preparado",
                "Leyendo configuración",
                "Escribiendo configuración",
                "Lengua",
                "Puntero",
                "Variable",
                "Referencia",
				"error de parsing",
                "memoria agotada",
                "variable ya existente",
                "id incorrecta",
                "max memoria debe ser > 16 y %16 == 0",
                "pos. de memoria incorrecta",
                "error de tipos; tipo incorrecto",
                "tipo desconocido",
                "variable desconocida",
                "durante la ejecuc.",
                "accediendo a la memoria en",
                "reservando memoria para",
                "char no permitido",
                "primer char debe ser letra",
                "se esperaba",
                "se esperaba literal o id",
                "uso incorrecto de derreferencia (\"*\")",
                "inesperado",
                "fin de sentencia",
                "las referencias deben ser inicializadas",
                "no es un puntero",
                "referencia sin inicializar",
                "referencia ya inicializada",
                "pos. de memoria incorrecta o no es del heap",
				"se esperaba comienzo de argumentos con '('",
				"se esperaba fin de argumentos con ')'",
				"no existe {0}() con {1} argumentos, necesita {2}.",
				"el argumento #{0}:'{1}' de {2}() debe ser del tipo {3}, y no del tipo {4}",
				"{0}() no encontrada",
				"se esperaba un número como: 0, 0.0 o 0x0",
				"no se puede acceder a void *",
				"faltan argumentos en llamada a fn."
    		}
		);

        /// <summary>
        /// Currently selected localized strings
        /// </summary>
		private static ReadOnlyCollection<string> strings = StringsEN;

        /// <summary>
        /// Sets the language (i."e.", the set of strings to use)
        /// </summary>
        /// <param name="locale">The <see cref="Locale"/> </param>
        /// <see cref="CultureInfo"/>
		public static void SetLanguage(CultureInfo locale)
		{
			if ( locale.TwoLetterISOLanguageName.ToUpper() == "ES" ) {
				strings = StringsES;
			}
			else
			if ( locale.TwoLetterISOLanguageName.ToUpper() == "EN" ) {
				strings = StringsEN;
			}

			return;
		}

        /// <summary>
        /// Returns a localized string, given its id.
        /// </summary>
		/// <param name="id">The <see cref="Id"/> for the string to get</param>
		public static string Get(Id id)
		{
			string toret = null;
			var numId = (int) id;

			if ( numId < strings.Count ) {
				toret = strings[ numId ];
			}

			return toret;
		}
	}
}
