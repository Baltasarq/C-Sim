using System;
using System.Globalization;
using System.Collections.ObjectModel;

namespace CSim.Core {
    /// <summary>
    /// L18n.
    /// Contains all strings needed in the app
    /// </summary>
	public static class L18n {

        /// Identifies all available strings
		public enum Id {
			ActReset,
			ActOpen,
			ActSave,
			ActInHex,
			ActInDec,
			ActZoomIn,
			ActZoomOut,
			ActShowMemory,
			ActShowDiagram,
			ActHelp,
			ActAbout,
			ActSettings,
			ActPlay,
			ActStop,
			StReady,
			StReadingConfig,
			StWritingConfig,
			LblLanguage,
            LblPointer,
            LblVble,
            LblRef,
			ExcParsing,
            ExcMemoryExhausted,
            ExcDuplicatedVble,
            ExcInvalidId,
            ExcInvalidMaxMemory,
            ExcInvalidMemory,
            ExcTypeMismatch,
            ExcUnknownType,
            ExcUnknownVble,
            ErrAccessingAt,
            ErrReserving,
            ErrNotAllowedChar,
            ErrFirstChar,
            ErrExpected,
            ErrExpectedLiteralOrId,
            ErrBadUseStar,
            ErrUnexpected,
            ErrEOL,
            ErrRefInit,
            ErrNotAPointer,
            ErrRefNotSet,
            ErrRefDoubleSet,
            ErrMemoryNotInHeap,
			ErrExpectedParametersBegin,
			ErrExpectedParametersEnd,
			ErrParamCount,
			ErrParamType,
			ErrFunctionNotFound,
			ErrExpectedLiteralNumber,
			ErrDerreferencedVoid,
			ErrMissingArguments,
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
				"About...",
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
				"{0}() does not exist with {1} arguments.",
				"param #{0}:'{1}' for {2}() should be of type {3}, instead of {4}",
				"function {0}() was not found",
				"expected number as in: 0, 0.0 or 0x0",
				"tried to derreference void *",
				"not enough arguments in function call",
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
                "error de tipos; se esperaba tipo",
                "tipo desconocido",
                "variable desconocida",
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
				"no existe {0}() con {1} argumentos.",
				"el argumento #{0}:'{1}' de {2}() debe ser del tipo {3}, y no del tipo {4}",
				"{0}() no encontrada",
				"se esperaba un número como: 0, 0.0 o 0x0",
				"no se puede acceder a void *",
				"faltan argumentos en llamada a fn.",
			}
		);

        /// <summary>
        /// Currently selected localized strings
        /// </summary>
		private static ReadOnlyCollection<string> strings = StringsEN;

        /// <summary>
        /// Sets the language (i.e., the set of strings to use)
        /// </summary>
        /// <param name="locale">Locale.</param>
        /// <see cref="System.Globalization.CultureInfo"/>
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
        /// <param name="id">The id for the string to get</param>
        /// <see cref="CSim.Core.L18n.Id"/>
		public static string Get(Id id)
		{
			string toret = null;
			int numId = (int) id;

			if ( numId < strings.Count ) {
				toret = strings[ numId ];
			}

			return toret;
		}
	}
}
