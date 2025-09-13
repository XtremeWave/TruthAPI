//using Hazel;
//using System.Reflection;

//namespace TruthAPI.CustomRpc;

////From Nebula On the Ship by Dolly
//[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
//public class TruthRPCHolder : Attribute
//{

//}

//public class TruthRPCInvoker
//{

//    Action<MessageWriter> sender;
//    Action localBodyProcess;
//    int hash;

//    public TruthRPCInvoker(int hash, Action<MessageWriter> sender, Action localBodyProcess)
//    {
//        this.hash = hash;
//        this.sender = sender;
//        this.localBodyProcess = localBodyProcess;
//    }

//    public void Invoke(MessageWriter writer)
//    {
//        writer.Write(hash);
//        sender.Invoke(writer);
//        localBodyProcess.Invoke();
//    }
//}

//public class RemoteProcessBase
//{
//    static public Dictionary<int, RemoteProcessBase> AllProcess = new();


//    public int Hash { get; private set; } = -1;
//    public string Name { get; private set; }

//    private const int MulPrime = 127;
//    private const int SurPrime = 104729;

//    public RemoteProcessBase(string name)
//    {
//        int val = 0;
//        int mul = 1;
//        foreach (char c in name)
//        {
//            mul *= MulPrime;
//            mul %= SurPrime;
//            val += (int)c * mul;
//            val %= SurPrime;
//        }
//        Hash = val;
//        Name = name;

//        if (AllProcess.ContainsKey(Hash)) Info($"Identifier conflict has been occured at {Name}", "TruthRPC");
//        AllProcess[Hash] = this;
//    }

//    static public void Load()
//    {
//        var types = Assembly.GetAssembly(typeof(RemoteProcessBase))?.GetTypes().Where((type) => type.IsDefined(typeof(TruthRPCHolder)));
//        if (types == null) return;

//        foreach (var type in types)
//            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
//    }

//    public virtual void Receive(MessageReader reader) { }
//}


//public class RemoteProcess<Parameter> : RemoteProcessBase
//{
//    public delegate void Process(Parameter parameter, bool isCalledByMe);

//    private Action<MessageWriter, Parameter> Sender { get; set; }
//    private Func<MessageReader, Parameter> Receiver { get; set; }
//    private Process Body { get; set; }

//    public RemoteProcess(string name, Action<MessageWriter, Parameter> sender, Func<MessageReader, Parameter> receiver, RemoteProcess<Parameter>.Process process)
//    : base(name)
//    {
//        Sender = sender;
//        Receiver = receiver;
//        Body = process;
//    }


//    public void Invoke(Parameter parameter)
//    {
//        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 64, Hazel.SendOption.Reliable, -1);
//        writer.Write(Hash);
//        Sender(writer, parameter);
//        AmongUsClient.Instance.FinishRpcImmediately(writer);
//        Body.Invoke(parameter, true);
//    }

//    public TruthRPCInvoker GetInvoker(Parameter parameter)
//    {
//        return new TruthRPCInvoker(Hash, (writer) => Sender(writer, parameter), () => Body.Invoke(parameter, true));
//    }

//    public void LocalInvoke(Parameter parameter)
//    {
//        Body.Invoke(parameter, true);
//    }

//    public override void Receive(MessageReader reader)
//    {
//        Body.Invoke(Receiver.Invoke(reader), false);
//    }
//}

//[TruthRPCHolder]
//public class CombinedRemoteProcess : RemoteProcessBase
//{
//    public static CombinedRemoteProcess CombinedRPC = new();
//    CombinedRemoteProcess() : base("CombinedRPC") { }

//    public override void Receive(MessageReader reader)
//    {
//        int num = reader.ReadInt32();
//        for (int i = 0; i < num; i++) RemoteProcessBase.AllProcess[reader.ReadInt32()].Receive(reader);
//    }

//    public void Invoke(params TruthRPCInvoker[] invokers)
//    {
//        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 64, Hazel.SendOption.Reliable, -1);
//        writer.Write(Hash);
//        writer.Write(invokers.Length);
//        foreach (var invoker in invokers) invoker.Invoke(writer);
//        AmongUsClient.Instance.FinishRpcImmediately(writer);
//    }
//}

//public class RemoteProcess : RemoteProcessBase
//{
//    public delegate void Process(bool isCalledByMe);
//    private Process Body { get; set; }
//    public RemoteProcess(string name, Process process)
//    : base(name)
//    {
//        Body = process;
//    }

//    public void Invoke()
//    {
//        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 64, Hazel.SendOption.Reliable, -1);
//        writer.Write(Hash);
//        AmongUsClient.Instance.FinishRpcImmediately(writer);
//        Body.Invoke(true);
//    }

//    public TruthRPCInvoker GetInvoker()
//    {
//        return new TruthRPCInvoker(Hash, (writer) => { }, () => Body.Invoke(true));
//    }

//    public override void Receive(MessageReader reader)
//    {
//        Body.Invoke(false);
//    }
//}

//public class DivisibleRemoteProcess<Parameter, DividedParameter> : RemoteProcessBase
//{
//    public delegate void Process(DividedParameter parameter, bool isCalledByMe);

//    private Action<Parameter, Action<DividedParameter>> Sender;
//    private Action<MessageWriter, DividedParameter> DividedSender { get; set; }
//    private Func<MessageReader, DividedParameter> Receiver { get; set; }
//    private Process Body { get; set; }

//    public DivisibleRemoteProcess(string name, Action<Parameter, Action<DividedParameter>> sender, Action<MessageWriter, DividedParameter> dividedSender, Func<MessageReader, DividedParameter> receiver, DivisibleRemoteProcess<Parameter, DividedParameter>.Process process)
//    : base(name)
//    {
//        Sender = sender;
//        DividedSender = dividedSender;
//        Receiver = receiver;
//        Body = process;
//    }

//    public void Invoke(Parameter parameter)
//    {
//        void dividedSend(DividedParameter param)
//        {
//            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, 64, Hazel.SendOption.Reliable, -1);
//            writer.Write(Hash);
//            DividedSender(writer, param);
//            AmongUsClient.Instance.FinishRpcImmediately(writer);
//            Body.Invoke(param, true);
//        }
//        Sender(parameter, dividedSend);
//    }

//    public void LocalInvoke(Parameter parameter)
//    {
//        Sender(parameter, (param) => Body.Invoke(param, true));
//    }

//    public override void Receive(MessageReader reader)
//    {
//        Body.Invoke(Receiver.Invoke(reader), false);
//    }
//}

//[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
//class TruthRPCHandlerPatch
//{
//    static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
//    {
//        if (callId != 64) return;

//        RemoteProcessBase.AllProcess[reader.ReadInt32()].Receive(reader);
//    }
//}

