using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YAMEP {

    public abstract class SymbolTableEntry {

        public enum EntryType {
            Variable,
            Function
        }

        public string IdentiferName { get; }
        public EntryType Type { get; }

        protected SymbolTableEntry( string name, EntryType entryType ) {
            IdentiferName = name;
            Type = entryType;
        }
    }

    public class VariableSymbolTableEntry : SymbolTableEntry {
        public double Value { get; set; }
        public VariableSymbolTableEntry( string name, double value )
            : base( name, EntryType.Variable ) => Value = value;
    }

    public class FunctionSystemTableEntry : SymbolTableEntry {
        public MethodInfo MethodInfo { get; }
        public FunctionSystemTableEntry( MethodInfo mi )
            : base( mi.Name, EntryType.Function ) => MethodInfo = mi;
    }

    public class SymbolTable {

        readonly Dictionary<string, SymbolTableEntry> Entries = new Dictionary<string, SymbolTableEntry>();

        public void AddOrUpdate( object variables ) {
            var kvp = variables.GetType( )
                .GetProperties( )
                .Select<PropertyInfo, (string, double)>( x => (x.Name, Convert.ToDouble( x.GetValue( variables ) )));

            foreach( var (id, val) in kvp )
                AddOrUpdate( id, val );
        }

        public void AddOrUpdate( string identifier, double value ) {
            var key = identifier.ToLower();
            if( !Entries.ContainsKey( key ) ) {
                Entries.Add( key, new VariableSymbolTableEntry( identifier, value ) );
            } else {
                var entry = Entries[key];
                if( entry.Type != SymbolTableEntry.EntryType.Variable )
                    throw new Exception( $"Identifier {identifier} type mismatch" );
                ( entry as VariableSymbolTableEntry ).Value = value;
            }
        }

        public void AddFunctions<T>( ) {
            //Get all Public Static Functions that return double take double parameter types
            //1 -  must have double return type
            //2 - parameters must be of type doulbe
            var methods = typeof(T)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where( mi => typeof(double).IsAssignableFrom(mi.ReturnType))
                .Where( mi => !mi.GetParameters().Any( p => !p.ParameterType.IsAssignableFrom(typeof(double))) );

            foreach( var mi in methods )
                Entries.Add( mi.Name.ToLower( ), new FunctionSystemTableEntry( mi ) );
        }


        public SymbolTableEntry Get( string idenifierName )
            => Entries.ContainsKey( idenifierName.ToLower( ) )
            ? Entries[ idenifierName.ToLower( ) ] : null;

    }
}
