using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAC.Components
{
    public class GlobalIdentifier
    {
        public GlobalIdentifier(string preferredIdentifier)
        {
            PreferredIdentifier = preferredIdentifier;
        }
        public string PreferredIdentifier { get; }
        public string? Identifier { get; set; }
        public override string ToString()
        {
            return Identifier ?? "UNSET_IDENTIFIER";
        }

        public static implicit operator string(GlobalIdentifier identifier)
        {
            return identifier.ToString();
        }

        public static implicit operator GlobalIdentifier(string identifier)
        {
            return new(identifier);
        }

        public static void ThrowIfUnset(params GlobalIdentifier[] ids)
        {
            foreach (GlobalIdentifier id in ids)
            {
                ArgumentNullException.ThrowIfNull(id);
            }
        }
    }
}
