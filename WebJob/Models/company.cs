using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJob.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("company")]
    public partial class company
    {
        public bool isFollowing = false;
    }
}