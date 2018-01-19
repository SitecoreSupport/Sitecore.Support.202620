﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Cintel.Reporting.DTOs;
using Sitecore.Cintel.Reporting.Services;
using Sitecore.Cintel.Reporting.Services.Mappers;
using Sitecore.Diagnostics;
using Sitecore.Cintel.Reporting;

namespace Sitecore.Support.Cintel.Reporting.Processors
{
  public class LatestEventsReportProcessor
  {
    public LatestEventsReportProcessor()
        : this(ViewFactory.GetLatestEventsReportService(), ViewFactory.GetEventToDataTableMapper())
    {
    }

    public LatestEventsReportProcessor(IEventsReportService service, IMapper<List<EventDTO>, DataTable> mapper)
    {
      Assert.ArgumentNotNull(service, nameof(service));
      Assert.ArgumentNotNull(mapper, nameof(mapper));
      Service = service;
      Mapper = mapper;
    }
    private IEventsReportService Service { get; }

    private IMapper<List<EventDTO>, DataTable> Mapper { get; }

    public void Process(ReportProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      var contactId = args.ReportParameters.ContactId;
      Guid interactionId;

      List<EventDTO> data;
      if (!string.IsNullOrEmpty(args.ReportParameters.ViewEntityId) &&
          Guid.TryParse(args.ReportParameters.ViewEntityId, out interactionId))
        data = Service.GetReport(new EventsReportServiceParameters(contactId, interactionId));
      else
        data = Service.GetReport(new EventsReportServiceParameters(contactId));
      
      #region Fix issue # 202620
      data.RemoveAll(x => (x.RelatedEvent != null && x.RelatedEvent.DefinitionId == Sitecore.XConnect.Collection.Model.PageViewEvent.EventDefinitionId));
      #endregion

      args.ResultTableForView = Mapper.Map(data);
    }
  }
}
