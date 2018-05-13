using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Chinook_dbo
 {
		[Table("Album", Schema="dbo")]
		public partial class Album {

			#region "constructor"
			public Album() {
				List_Track_AlbumId = new HashSet<Track>();
			}
			#endregion
			#region "columns Album"

			[Key, Column(name:"AlbumId", Order = 0) ]
			[Required]
			public int AlbumId { get; set; }

			[Column(name:"Title")]
			[Required]
			[StringLength(160)]
			public string Title { get; set; }

			[Column(name:"ArtistId")]
			[Required]
			public int ArtistId { get; set; }
			#endregion

			#region "foreign keys"
			[ForeignKey("ArtistId")]
			public Artist FK_AlbumArtistId { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_TrackAlbumId")]
			public virtual ICollection<Track> List_Track_AlbumId { get; set; }
			#endregion
		 }
		[Table("Artist", Schema="dbo")]
		public partial class Artist {

			#region "constructor"
			public Artist() {
				List_Album_ArtistId = new HashSet<Album>();
			}
			#endregion
			#region "columns Artist"

			[Key, Column(name:"ArtistId", Order = 0) ]
			[Required]
			public int ArtistId { get; set; }

			[Column(name:"Name")]
			[StringLength(120)]
			public string Name { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_AlbumArtistId")]
			public virtual ICollection<Album> List_Album_ArtistId { get; set; }
			#endregion
		 }
		[Table("Customer", Schema="dbo")]
		public partial class Customer {

			#region "constructor"
			public Customer() {
				List_Invoice_CustomerId = new HashSet<Invoice>();
			}
			#endregion
			#region "columns Customer"

			[Key, Column(name:"CustomerId", Order = 0) ]
			[Required]
			public int CustomerId { get; set; }

			[Column(name:"FirstName")]
			[Required]
			[StringLength(40)]
			public string FirstName { get; set; }

			[Column(name:"LastName")]
			[Required]
			[StringLength(20)]
			public string LastName { get; set; }

			[Column(name:"Company")]
			[StringLength(80)]
			public string Company { get; set; }

			[Column(name:"Address")]
			[StringLength(70)]
			public string Address { get; set; }

			[Column(name:"City")]
			[StringLength(40)]
			public string City { get; set; }

			[Column(name:"State")]
			[StringLength(40)]
			public string State { get; set; }

			[Column(name:"Country")]
			[StringLength(40)]
			public string Country { get; set; }

			[Column(name:"PostalCode")]
			[StringLength(10)]
			public string PostalCode { get; set; }

			[Column(name:"Phone")]
			[StringLength(24)]
			public string Phone { get; set; }

			[Column(name:"Fax")]
			[StringLength(24)]
			public string Fax { get; set; }

			[Column(name:"Email")]
			[Required]
			[StringLength(60)]
			public string Email { get; set; }

			[Column(name:"SupportRepId")]
			public int? SupportRepId { get; set; }
			#endregion

			#region "foreign keys"
			[ForeignKey("SupportRepId")]
			public Employee FK_CustomerSupportRepId { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_InvoiceCustomerId")]
			public virtual ICollection<Invoice> List_Invoice_CustomerId { get; set; }
			#endregion
		 }
		[Table("Employee", Schema="dbo")]
		public partial class Employee {

			#region "constructor"
			public Employee() {
				List_Customer_SupportRepId = new HashSet<Customer>();
				List_Employee_ReportsTo = new HashSet<Employee>();
			}
			#endregion
			#region "columns Employee"

			[Key, Column(name:"EmployeeId", Order = 0) ]
			[Required]
			public int EmployeeId { get; set; }

			[Column(name:"LastName")]
			[Required]
			[StringLength(20)]
			public string LastName { get; set; }

			[Column(name:"FirstName")]
			[Required]
			[StringLength(20)]
			public string FirstName { get; set; }

			[Column(name:"Title")]
			[StringLength(30)]
			public string Title { get; set; }

			[Column(name:"ReportsTo")]
			public int? ReportsTo { get; set; }

			[Column(name:"BirthDate")]
			public System.DateTime? BirthDate { get; set; }

			[Column(name:"HireDate")]
			public System.DateTime? HireDate { get; set; }

			[Column(name:"Address")]
			[StringLength(70)]
			public string Address { get; set; }

			[Column(name:"City")]
			[StringLength(40)]
			public string City { get; set; }

			[Column(name:"State")]
			[StringLength(40)]
			public string State { get; set; }

			[Column(name:"Country")]
			[StringLength(40)]
			public string Country { get; set; }

			[Column(name:"PostalCode")]
			[StringLength(10)]
			public string PostalCode { get; set; }

			[Column(name:"Phone")]
			[StringLength(24)]
			public string Phone { get; set; }

			[Column(name:"Fax")]
			[StringLength(24)]
			public string Fax { get; set; }

			[Column(name:"Email")]
			[StringLength(60)]
			public string Email { get; set; }
			#endregion

			#region "foreign keys"
			[ForeignKey("ReportsTo")]
			public Employee FK_EmployeeReportsTo { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_CustomerSupportRepId")]
			public virtual ICollection<Customer> List_Customer_SupportRepId { get; set; }
			[InverseProperty("FK_EmployeeReportsTo")]
			public virtual ICollection<Employee> List_Employee_ReportsTo { get; set; }
			#endregion
		 }
		[Table("Genre", Schema="dbo")]
		public partial class Genre {

			#region "constructor"
			public Genre() {
				List_Track_GenreId = new HashSet<Track>();
			}
			#endregion
			#region "columns Genre"

			[Key, Column(name:"GenreId", Order = 0) ]
			[Required]
			public int GenreId { get; set; }

			[Column(name:"Name")]
			[StringLength(120)]
			public string Name { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_TrackGenreId")]
			public virtual ICollection<Track> List_Track_GenreId { get; set; }
			#endregion
		 }
		[Table("Invoice", Schema="dbo")]
		public partial class Invoice {

			#region "constructor"
			public Invoice() {
				List_InvoiceLine_InvoiceId = new HashSet<InvoiceLine>();
			}
			#endregion
			#region "columns Invoice"

			[Key, Column(name:"InvoiceId", Order = 0) ]
			[Required]
			public int InvoiceId { get; set; }

			[Column(name:"CustomerId")]
			[Required]
			public int CustomerId { get; set; }

			[Column(name:"InvoiceDate")]
			[Required]
			public System.DateTime InvoiceDate { get; set; }

			[Column(name:"BillingAddress")]
			[StringLength(70)]
			public string BillingAddress { get; set; }

			[Column(name:"BillingCity")]
			[StringLength(40)]
			public string BillingCity { get; set; }

			[Column(name:"BillingState")]
			[StringLength(40)]
			public string BillingState { get; set; }

			[Column(name:"BillingCountry")]
			[StringLength(40)]
			public string BillingCountry { get; set; }

			[Column(name:"BillingPostalCode")]
			[StringLength(10)]
			public string BillingPostalCode { get; set; }

			[Column(name:"Total")]
			[Required]
			public System.Decimal Total { get; set; }
			#endregion

			#region "foreign keys"
			[ForeignKey("CustomerId")]
			public Customer FK_InvoiceCustomerId { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_InvoiceLineInvoiceId")]
			public virtual ICollection<InvoiceLine> List_InvoiceLine_InvoiceId { get; set; }
			#endregion
		 }
		[Table("InvoiceLine", Schema="dbo")]
		public partial class InvoiceLine {
			#region "columns InvoiceLine"

			[Key, Column(name:"InvoiceLineId", Order = 0) ]
			[Required]
			public int InvoiceLineId { get; set; }

			[Column(name:"InvoiceId")]
			[Required]
			public int InvoiceId { get; set; }

			[Column(name:"TrackId")]
			[Required]
			public int TrackId { get; set; }

			[Column(name:"UnitPrice")]
			[Required]
			public System.Decimal UnitPrice { get; set; }

			[Column(name:"Quantity")]
			[Required]
			public int Quantity { get; set; }
			#endregion

			#region "foreign keys"
			[ForeignKey("InvoiceId")]
			public Invoice FK_InvoiceLineInvoiceId { get; set; }
			[ForeignKey("TrackId")]
			public Track FK_InvoiceLineTrackId { get; set; }
			#endregion
		 }
		[Table("MediaType", Schema="dbo")]
		public partial class MediaType {

			#region "constructor"
			public MediaType() {
				List_Track_MediaTypeId = new HashSet<Track>();
			}
			#endregion
			#region "columns MediaType"

			[Key, Column(name:"MediaTypeId", Order = 0) ]
			[Required]
			public int MediaTypeId { get; set; }

			[Column(name:"Name")]
			[StringLength(120)]
			public string Name { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_TrackMediaTypeId")]
			public virtual ICollection<Track> List_Track_MediaTypeId { get; set; }
			#endregion
		 }
		[Table("Playlist", Schema="dbo")]
		public partial class Playlist {

			#region "constructor"
			public Playlist() {
				List_PlaylistTrack_PlaylistId = new HashSet<PlaylistTrack>();
			}
			#endregion
			#region "columns Playlist"

			[Key, Column(name:"PlaylistId", Order = 0) ]
			[Required]
			public int PlaylistId { get; set; }

			[Column(name:"Name")]
			[StringLength(120)]
			public string Name { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_PlaylistTrackPlaylistId")]
			public virtual ICollection<PlaylistTrack> List_PlaylistTrack_PlaylistId { get; set; }
			#endregion
		 }
		[Table("PlaylistTrack", Schema="dbo")]
		public partial class PlaylistTrack {
			#region "columns PlaylistTrack"

			[Key, Column(name:"PlaylistId", Order = 0) ]
			[Required]
			public int PlaylistId { get; set; }

			[Key, Column(name:"TrackId", Order = 1) ]
			[Required]
			public int TrackId { get; set; }
			#endregion

			#region "foreign keys"
			[ForeignKey("PlaylistId")]
			public Playlist FK_PlaylistTrackPlaylistId { get; set; }
			[ForeignKey("TrackId")]
			public Track FK_PlaylistTrackTrackId { get; set; }
			#endregion
		 }
		[Table("sysdiagrams", Schema="dbo")]
		public partial class sysdiagrams {
			#region "columns sysdiagrams"

			[Column(name:"name")]
			[Required]
			[StringLength(128)]
			public string name { get; set; }

			[Column(name:"principal_id")]
			[Required]
			public int principal_id { get; set; }

			[Key, Column(name:"diagram_id", Order = 0) ]
			[Required]
			public int diagram_id { get; set; }

			[Column(name:"version")]
			public int? version { get; set; }

			[Column(name:"definition")]
			public System.Byte?[] definition { get; set; }
			#endregion
		 }
		[Table("Track", Schema="dbo")]
		public partial class Track {

			#region "constructor"
			public Track() {
				List_InvoiceLine_TrackId = new HashSet<InvoiceLine>();
				List_PlaylistTrack_TrackId = new HashSet<PlaylistTrack>();
			}
			#endregion
			#region "columns Track"

			[Key, Column(name:"TrackId", Order = 0) ]
			[Required]
			public int TrackId { get; set; }

			[Column(name:"Name")]
			[Required]
			[StringLength(200)]
			public string Name { get; set; }

			[Column(name:"AlbumId")]
			public int? AlbumId { get; set; }

			[Column(name:"MediaTypeId")]
			[Required]
			public int MediaTypeId { get; set; }

			[Column(name:"GenreId")]
			public int? GenreId { get; set; }

			[Column(name:"Composer")]
			[StringLength(220)]
			public string Composer { get; set; }

			[Column(name:"Milliseconds")]
			[Required]
			public int Milliseconds { get; set; }

			[Column(name:"Bytes")]
			public int? Bytes { get; set; }

			[Column(name:"UnitPrice")]
			[Required]
			public System.Decimal UnitPrice { get; set; }
			#endregion

			#region "foreign keys"
			[ForeignKey("AlbumId")]
			public Album FK_TrackAlbumId { get; set; }
			[ForeignKey("GenreId")]
			public Genre FK_TrackGenreId { get; set; }
			[ForeignKey("MediaTypeId")]
			public MediaType FK_TrackMediaTypeId { get; set; }
			#endregion

			#region "inversion"
			[InverseProperty("FK_InvoiceLineTrackId")]
			public virtual ICollection<InvoiceLine> List_InvoiceLine_TrackId { get; set; }
			[InverseProperty("FK_PlaylistTrackTrackId")]
			public virtual ICollection<PlaylistTrack> List_PlaylistTrack_TrackId { get; set; }
			#endregion
		 }
 }
