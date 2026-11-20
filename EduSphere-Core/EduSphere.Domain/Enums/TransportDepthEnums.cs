namespace EduSphere.Domain.Enums;

public enum TransportCrewRole { Driver = 1, Conductor = 2, Attendant = 3 }
public enum VehicleDocumentType { Insurance = 1, Permit = 2, PollutionCertificate = 3, Registration = 4, Fitness = 5, Other = 99 }
public enum VehicleDocumentStatus { Valid = 1, ExpiringSoon = 2, Expired = 3, Replaced = 4 }
public enum GeofenceEventType { Entered = 1, Exited = 2, Arriving = 3 }
public enum TransportAttendanceEvent { Boarded = 1, Alighted = 2, Rejected = 3 }
public enum TransportScanMethod { Qr = 1, Rfid = 2, Manual = 3 }
public enum RouteOptimizationStatus { Requested = 1, Running = 2, Completed = 3, Failed = 4, Applied = 5 }
public enum GpsProviderType { GenericWebhook = 1, Traccar = 2, FleetStack = 3, CustomHttp = 99 }
