using System.ComponentModel.DataAnnotations;

namespace AddData.Database.UniversalData
{
	public class ConnectorInfo
	{
		[Key]
		public string		PeerUUID			{ get; set; }

		public int			InstanceStartTime	{ get; set; }
		public string		UpdateSeq			{ get; set; }
		public string		Version				{ get; set; }
		public VendorInfo	Vendor				{ get; set; }
	}
}
