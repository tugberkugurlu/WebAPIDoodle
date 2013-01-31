using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiDoodle.Net.Http.Client.Sample45.Validation;

namespace WebApiDoodle.Net.Http.Client.Sample45.RequestCommands {

    public class PaginatedRequestCommand : IRequestCommand {

        public PaginatedRequestCommand() {
        }

        public PaginatedRequestCommand(int page, int take) {
            Page = page;
            Take = take;
        }

        [Minimum(1)]
        public int Page { get; set; }

        [Maximum(50)]
        [Minimum(1)]
        public int Take { get; set; }
    }
}