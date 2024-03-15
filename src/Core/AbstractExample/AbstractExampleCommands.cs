using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.AbstractExample
{
    [DataContract]
    [KnownType(typeof(CreateAbstractExample))]
    [KnownType(typeof(ApplyAbstractExample))]
    [KnownType(typeof(UpdateAbstractExample))]
    [KnownType(typeof(RejectAbstractExample))]
    [KnownType(typeof(ApproveAbstractExample))]
    public class ApplicationCommand
    {
        public Guid Id { get; init; }

        public DateTime DateRequestedUtc { get; init; }

        public CommandStateCode CurrentState { get; set; }

        public string? FailureException { get; set; }

        // Default constructor for derived classes
        protected ApplicationCommand()
        {
            DateRequestedUtc = DateTime.UtcNow;
            Id = Guid.NewGuid();
            CurrentState = CommandStateCode.Processing;
        }
    }

    public enum CommandStateCode
    {
        Accepted,
        Rejected,
        Processing,
        Success,
        Failure
    }

    public class CommandResult
    {
        private CommandResult()
        {
        }

        private CommandResult( string failureReason )
        {
            FailureReason = failureReason;
        }

        public string FailureReason { get; } = "";

        public bool IsSuccess => string.IsNullOrEmpty( FailureReason );
    
        public static CommandResult Success { get; } = new();

        public static CommandResult Fail( string reason )
        {
            return new CommandResult( reason );
        }
    }

    [DataContract]
    public abstract class AbstractExampleCommand : ApplicationCommand
    {
        [DataMember]
        public Guid AbstractExampleId { get; init; }

        protected AbstractExampleCommand(Guid abstractExampleId)
        {
            AbstractExampleId = abstractExampleId;
        }
        
    }

    [DataContract]
    public class CreateAbstractExample : AbstractExampleCommand
    {
        [DataMember]
        public Guid SourceBranchId { get; init; }

        [DataMember]
        public Guid TargetBranchId { get; init; }

        [DataMember]
        public List<Guid> CorrelationIdList { get; init; }

        public CreateAbstractExample(Guid sourceBranchId, Guid targetBranchId, List<Guid> correlationIdList, Guid abstractExampleId )
            : base(abstractExampleId)
        {
            SourceBranchId = sourceBranchId;
            TargetBranchId = targetBranchId;
            CorrelationIdList = correlationIdList;
        }
        
        public static CreateAbstractExample? FromJson(
            string json)
        {
            return JsonConvert.DeserializeObject<CreateAbstractExample>(json, Converter.Settings);
        }
        
    }

    [DataContract]
    public class ApplyAbstractExample : AbstractExampleCommand
    {
        public ApplyAbstractExample(Guid abstractExampleId)
            : base(abstractExampleId)
        {
        }
        
        public static ApplyAbstractExample? FromJson(
            string json)
        {
            return JsonConvert.DeserializeObject<ApplyAbstractExample>(json, Converter.Settings);
        }
        
        public static string ToJson(ApplyAbstractExample command) => JsonConvert.SerializeObject((object) command, Converter.Settings);
    }

    [DataContract]
    public class ApproveAbstractExample : AbstractExampleCommand
    {
        public ApproveAbstractExample(Guid abstractExampleId)
            : base(abstractExampleId)
        {
        }
        public static ApproveAbstractExample? FromJson(
            string json)
        {
            return JsonConvert.DeserializeObject<ApproveAbstractExample>(json, Converter.Settings);
        }
    }

    [DataContract]
    public class RejectAbstractExample : AbstractExampleCommand
    {
        public RejectAbstractExample(Guid abstractExampleId)
            : base(abstractExampleId)
        {
        }
        public static RejectAbstractExample? FromJson(
            string json)
        {
            return JsonConvert.DeserializeObject<RejectAbstractExample>(json, Converter.Settings);
        }
    }

    [DataContract]
    public class UpdateAbstractExample : AbstractExampleCommand
    {
        [DataMember]
        public List<Guid> CorrelationIdList { get; init; }

        public UpdateAbstractExample(List<Guid> correlationIdList, Guid abstractExampleId)
            : base(abstractExampleId)
        {
            CorrelationIdList = correlationIdList;
        }
        
        public static UpdateAbstractExample? FromJson(
            string json)
        {
            return JsonConvert.DeserializeObject<UpdateAbstractExample>(json, Converter.Settings);
        }
    }
   
    public static class SerializeCommand
    {
        public static string ToJson(this CreateAbstractExample self) => JsonConvert.SerializeObject((object) self, Converter.Settings);
        public static string ToJson(this ApplyAbstractExample self) => JsonConvert.SerializeObject((object) self, Converter.Settings);

        public static string ToJson(this ApproveAbstractExample self) => JsonConvert.SerializeObject((object) self, Converter.Settings);
        
        public static string ToJson(this RejectAbstractExample self) => JsonConvert.SerializeObject((object) self, Converter.Settings);
        public static string ToJson(this UpdateAbstractExample self) => JsonConvert.SerializeObject((object) self, Converter.Settings);

    }
    
    
    internal static class Converter
    {
        public static readonly JsonSerializerSettings? Settings;

        static Converter()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None
            };
            serializerSettings.Converters.Add((JsonConverter) new IsoDateTimeConverter()
            {
                DateTimeStyles = DateTimeStyles.AssumeUniversal
            });
            Settings = serializerSettings;
        }
    }
}
