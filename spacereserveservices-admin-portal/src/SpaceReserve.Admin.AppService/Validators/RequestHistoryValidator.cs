
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SpaceReserve.Admin.AppService.DTOs;

namespace SpaceReserve.Admin.AppService.Validator
{
    public class RequestHistoryValidator : AbstractValidator<RequestHistoryQueryParamDto>
    {
        [Obsolete]
        public RequestHistoryValidator()
        {
            CascadeMode = CascadeMode.Stop;
            
            RuleFor(x => x.Sort)
                .Must(x => x == 0 || x == 1 || x == 2 || x == 3 || x == 4)
                .WithMessage("Sort order must be 0, 1, 2, 3, or 4.");
        }
    }
}
