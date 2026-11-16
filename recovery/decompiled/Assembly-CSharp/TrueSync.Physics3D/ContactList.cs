using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class ContactList : List<Contact>
{
	public ContactList()
		: base(4)
	{
	}
}
