// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
	/// <summary>The engine's exception, parent for all the rest.</summary>
	public abstract class EngineException: System.Exception {
		
		/// <summary>
		/// Initializes a new <see cref="T:CSim.Core.EngineException"/>.
		/// </summary>
		/// <param name="s">The detailed message.</param>
		protected EngineException(string s): base( s )
		{
		}
	}
    
    namespace Exceptions {
    /// <summary>Runtime exception: errors produced during execution.</summary>
    public class RuntimeException: EngineException {
        /// <summary>
        /// Initializes a new <see cref="T:RuntimeException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public RuntimeException(string s)
            : base( L10n.Get( L10n.Id.ExcRuntime ) + ": " + s )
        {
        }
    }
    
    /// <summary> Already existing vble exception./// </summary>
    public class AlreadyExistingVbleException: RuntimeException {
        /// <summary>
        /// Initializes a new <see cref="T:AlreadyExistingVbleException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public AlreadyExistingVbleException(string s)
            : base( L10n.Get( L10n.Id.ExcDuplicatedVble ) + ": " + s )
        {
        }
    }
    
    /// <summary>Exhausted memory exception.</summary>
    public class ExhaustedMemoryException: RuntimeException {
        /// <summary>
        /// Initializes a new <see cref="T:ExhaustedMemoryException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public ExhaustedMemoryException(string s)
            : base( L10n.Get( L10n.Id.ExcMemoryExhausted ) + ": " + s )
        {
        }
    }
    
    /// <summary>Incorrect address exception.</summary>
    public class IncorrectAddressException: EngineException {
        /// <summary>
        /// Initializes a new <see cref="T:IncorrectAddressException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public IncorrectAddressException(string s)
            :base( L10n.Get( L10n.Id.ExcInvalidMemory ) + ": " + s )
        {
        }
    }
    
    /// <summary>Invalid identifier exception.</summary>
    public class InvalidIdException: EngineException {
        /// <summary>
        /// Initializes a new <see cref="T:InvalidIdException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public InvalidIdException(string s)
            : base( L10n.Get( L10n.Id.ExcInvalidId ) + ": " + s )
        {
        }
    }
    
    /// <summary>Invalid max memory exception.</summary>
    public class InvalidMaxMemoryException: EngineException {
        /// <summary>
        /// Initializes a new <see cref="T:InvalidMaxMemoryException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public InvalidMaxMemoryException(string s)
            : base( L10n.Id.ExcInvalidMaxMemory + ": " + s )
        {
        }
    }
    
    /// <summary>Parsing exception.</summary>
    public class ParsingException: EngineException {
        /// <summary>
        /// Initializes a new <see cref="T:ParsingException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public ParsingException(string s)
            : base( L10n.Get( L10n.Id.ExcParsing ) + ": " + s )
        {
        }
    }
    
    /// <summary>Type mismatch exceptions.</summary>
    public class TypeMismatchException: EngineException {
        /// <summary>
        /// Initializes a new <see cref="T:TypeMismatchException"/>.
        /// </summary>
        /// <param name="msg">The detailed message.</param>
        public TypeMismatchException(string msg)
            : base( L10n.Get( L10n.Id.ExcTypeMismatch ) + ": " + msg )
        {
        }
    }
    
    /// <summary>Unknown type exception.</summary>
    public class UnknownTypeException: RuntimeException {
        /// <summary>
        /// Initializes a new <see cref="T:UnknownTypeException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public UnknownTypeException(string s)
            : base( L10n.Get( L10n.Id.ExcUnknownType ) + ": " + s )
        {
        }
    }
    
    /// <summary>Unknown vble exception.</summary>
    public class UnknownVbleException: RuntimeException {
        /// <summary>
        /// Initializes a new <see cref="T:UnknownVbleException"/>.
        /// </summary>
        /// <param name="s">The detailed message.</param>
        public UnknownVbleException(string s)
            : base( L10n.Get( L10n.Id.ExcUnknownVble ) + ": " + s )
        {
        }
    }
    }
}
