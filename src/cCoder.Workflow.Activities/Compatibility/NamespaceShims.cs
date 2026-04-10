// Namespace shims keep older workflow imports and serialized type names working
// while the concrete implementations live in the Workflow activities project.

namespace cCoder.Workflow.Activities.Sftp
{
    public abstract class SftpActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpActivity { }
    public class SftpCreateBinaryFilesActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpCreateBinaryFilesActivity { }
    public class SftpCreateTextFilesActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpCreateTextFilesActivity { }
    public class SftpDeleteActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpDeleteActivity { }
    public class SftpFetchActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpFetchActivity { }
    public class SftpMoveActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpMoveActivity { }
}

namespace cCoder.Core.Connectivity.Workflow.Sftp
{
    public abstract class SftpActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpActivity { }
    public class SftpCreateBinaryFilesActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpCreateBinaryFilesActivity { }
    public class SftpCreateTextFilesActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpCreateTextFilesActivity { }
    public class SftpDeleteActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpDeleteActivity { }
    public class SftpFetchActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpFetchActivity { }
    public class SftpMoveActivity : global::cCoder.Workflow.Activities.Activities.Sftp.SftpMoveActivity { }
}
