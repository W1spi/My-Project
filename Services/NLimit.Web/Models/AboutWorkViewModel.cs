using Data.NLimit.Common.EntitiesModels.SqlServer;
using System.ComponentModel.DataAnnotations;

namespace NLimit.Web.Models;

public class AboutWorkViewModel : Work
{
    public bool? WorkIsPresent { get; set; } // флаг присутствия заданий

    public IEnumerable<AboutWorkViewModel>? AllWorks { get; set; } 
}