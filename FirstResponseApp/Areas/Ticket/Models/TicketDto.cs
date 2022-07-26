using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirstResponseApp.Areas.Ticket.Models
{
    public class TicketDto
    {
        public long Id { get; set; }


        [Required]
        [Display(Name = "Waiting On")]
        public int OrganisationId { get; set; }
        public List<SelectListItem> OrganisationList { get; set; }

        [Required]
        [Display(Name = "Channel")]
        public long ChannelId { get; set; }

        [Display(Name = "Client Number")]//change by prashant MatterNumber
        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Only numbers are allowed")]
       // [RegularExpression("^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "Please Enter alphanumeric characters")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Minumum 5 and Maximum 15 characters allow")]
        public string MatterNumber { get; set; }

        public string MatterNumberView { get; set; }

        //public TbWaitingOnMaster WaitingOn { get; set; }
        //[ForeignKey("WaitingOn")]
        //public long WaitingOnId { get; set; }

        public long TicketMasterId { get; set; }

        [Required]
        [Display(Name = "Ticket Name")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allow")]
        public string TicketName { get; set; }

     
        [Display(Name = "Linfox Notes")]
       // [MaxLength(50, ErrorMessage = "Name cannot be greater than 50")]
        public string TicketNotes { get; set; }


        [Display(Name = "HWLE Notes")]
        public string HwleTicketNotes { get; set; }

        [Display(Name = "Created On")]
        public DateTime? CreatedOn { get; set; }

        public string CreatedByUserId { get; set; }
        public bool IsDeleted { get; set; }

        public string DeletedBy { get; set; }

        public bool IsActive { get; set; }

        
        public long? StatusId { get; set; }

        public List<SelectListItem> ChannelList { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Upload Document")]
        public IFormFile[] Documents { get; set; }

        [Display(Name = "Search Text")]
        public string SearchText { get; set; }

        [Display(Name = "Search Text")]
        public string SearchTextForClosed { get; set; }

        public string OrganisationName { get; set; }
        public string ChannelName { get; set; }
        public string CreatedByName { get; set; }

        public string AddedOn { get; set; }

        public string Linfoxtext { get; set; }
        public string MedilawText { get; set; }

        public string HWLEText { get; set; }

        [Display(Name = "Waiting On")]
        public long  WaitingOn { get; set; }
        public string  WaitingOnName { get; set; }

        public string EncryptedtTicketId { get; set; }

        public string LoggedInUserId { get; set; }

        public long LoggedInOrganisationId { get; set; }


        public string LastUpdatedOnHwle { get; set; }
        public string LastUpdatedOnLinfox { get; set; }
        public string LastUpdatedOnMedilow { get; set; }

        public string LastUpdatedByHwle { get; set; }
        public string LastUpdatedByLinfox { get; set; }
        public string LastUpdatedByMedilow { get; set; }
    }


    public class TicketModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public string WaitingOn { get; set; }
        public string AddedOn { get; set; }
        public string AddedBy { get; set; }

        public string ClosedBy { get; set; }
        public string ClosedOn { get; set; }

        public long ChannelId { get; set; }
        public long OrganisationId { get; set; }
        public DateTime CreatedOnDate { get; set; }

        public int ApplicationCount { get; set; }

        public string EncryptedTicketId { get; set; }
        public long LoggedInOrganisationId { get; set; }
    }

    public class TicketData
    {
        public TicketModel[] Data { get; set; }
        public int ApplicationCount { get; set; }
    }

    public class UserEmailValues
    {
        public string EmailAddress { get; set; }
        public string FullName { get; set; }

    }

    public class ActiveTicketSearchParameters
    {
        public string ChannelId { get; set; }
        public string OrganisationId { get; set; }
        public string CreatedOn { get; set; }
        public string TextSearch { get; set; }
    }

    public class DocumentListing
    {
        public long Id { get; set; }
        public string DocumentName { get; set; }
        public string AddedBy { get; set; }
        public string AddedOn { get; set; }

        public string Organisation { get; set; }

        public string CreatedByUserId { get; set; }

        public string IsCreatedByLoggedInuser { get; set; }

        public string TableType { get; set; }
    }

    public class DocumentData
    {
        public DocumentListing[] Data { get; set; }
        public int DocumentCount { get; set; }
    }

}
