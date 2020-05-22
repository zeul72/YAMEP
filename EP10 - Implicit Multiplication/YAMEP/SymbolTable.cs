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
            if( !Entries.ContainsKey( identifier ) ) {
                Entries.Add( identifier, new VariableSymbolTableEntry( identifier, value ) );
            } else {
                var entry = Entries[identifier];
                if( entry.Type != SymbolTableEntry.EntryType.Variable )
                    throw new Exception( $"Identifier {identifier} type mismatch" );
                ( entry as VariableSymbolTableEntry ).Value = value;
            }
        }

        public SymbolTableEntry Get( string idenifierName )
            => Entries.ContainsKey( idenifierName )
            ? Entries[ idenifierName ] : null;

    }
}
