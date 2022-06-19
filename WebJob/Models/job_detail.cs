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
    [Table("job_detail")]
    public partial class job_detail
    {
        public bool isFollowing = false;
        public bool isApply = false;
    }
}