/* tfwxo * 1/18/2016 * 9:57 PM */
using System;
using System.IO;
using on.iff;
using on.dsformat;

namespace on.drumsynth2
{
  /// <summary>
  /// Description of ds2wav.
  /// We should probably be using doble instead of floats.
  /// </summary>
  public partial class ds2wav
  {
    internal IDsPreset DsPgm { get; set; } = null;
    internal Stream mrStream { get; set; } = null;

    internal int BUFFER_SIZE  { get; set; } = 960;
    internal int BUFFER_SIZE2 { get; set; } // = BUFFER_SIZE*2;

    public static int ENV_MAX_COUNT = 256;
    public int EnvelopeCountMax { get; set; } = ENV_MAX_COUNT;
    
    #region Fields/Properties
    //readonly string[] Sections = {};
    #pragma warning disable 168, 219
    
    internal int Fs { get; set; }=44100; // 8096 11025 22050 44100 48000 92000;
    
    const double Pi      = (double)Math.PI; //6.285714f; //Math.PI;
    const double TwoPi   = Pi * 2;
    //const double TwoPi = 6.285714f;
    
    const int MAX    = 0; // envData[e,MAX] -> last known sample position
    const int ENV    = 1; // Total number of envpts?
    const int PNT    = 2; // current or active point in gen-process
    const int dENV   = 3; // delta (look-ahead or interpolated point?)
    const int NEXTT  = 4; // next point?

    int        busy;      // so its not used, eh?
    double[,,] envpts;
    double[,]  envData    = new double[8,6];
    // checkbox
    int[]     chkOn      = new int[8];
    int[]     sliLev     = new int[8];
    double    timestretch;
    // short[]   DD      = new short[BUFFER_SIZE];
    short     clippoint;
    short[]   DF;// ?
    double[]  phi; //

    /*----------------------------------------*/
    // short[]   wave;
    /// wavewords is within the buffered write process itself.
    /// wavewords is the number of audio blocks written.
    long wavewords;

    /// wavemode is within the buffered write process itself.
    /// if wavemode is 1, then a waveform is written to file.
    /// if not, a memory stream is created and a foux process runs.
    long wavemode;
    /*----------------------------------------*/
    double     mem_t     = 1.0f;
    double     mem_o     = 1.0f;
    double     mem_n     = 1.0f;
    double     mem_b     = 1.0f;
    double     mem_tune  = 1.0f;
    double     mem_time  = 1.0f;
    /*----------------------------------------*/
    #pragma warning restore 168, 219
    
    // there are 7 envelopes...
    // 1 = Tone 2 = Noise 3 = Overtones1 4 = Overtones2 5 = NoiseBand 6 = NoiseBand2 7 = Highpass (General)
    #endregion

    #region helper methods
    
    int LongestEnv()
    {
      long e, eon, p;
      double l = 0.0f;
      for (e = 1; e < 7; e++) // 3
      {
        eon = e-1;
        if (eon > 2) eon = eon-1;
        p=0;
        while (envpts[e,0,p+1] >= 0.0f) p++;
        envData[e,MAX] = envpts[e,0,p] * timestretch;
        if (chkOn[eon]==1 && envData[e,MAX] > l) l = envData[e,MAX];
      }
      // l*= timestretch;
      return BUFFER_SIZE2 + (BUFFER_SIZE * (int)(l / BUFFER_SIZE));
    }
    
    double LoudestEnv()
    {
      double loudest=0.0f;
      int i=0;

      while (i<5) //2
      {
        if(chkOn[i]==1 && sliLev[i]>loudest) loudest=(double)sliLev[i];
        i++;
      }
      return (loudest * loudest);
    }
    
    void UpdateEnv(int e, long t)
    {
      double endEnv, dT;
      long op = (long)envData[e,PNT];
      
      //0.2's added
      envData[e,NEXTT] = envpts[e,0,(long)(op+1.0f).Contain(0,EnvelopeCountMax-1)] * timestretch;       //get next point
      if(envData[e,NEXTT] < 0) envData[e,NEXTT] = 442000.0f * timestretch;   //if end point, hold
      
      var op2=(int)(envData[e,PNT] + 1.0f).Contain(0,EnvelopeCountMax-1);
      envData[e,ENV] = envpts[e,1,op] * 0.01f;                            //this level
      endEnv = envpts[e,1,op2] * 0.01f;         //next level
      
      dT = envData[e,NEXTT] - (double)t;
      if(dT < 1.0) dT = 1.0f;
      envData[e,dENV] = (endEnv - envData[e,ENV]) / dT;
      envData[e,PNT] = (envData[e,PNT] + 1.0f).Contain(0,EnvelopeCountMax-1);
    }

    void getParseEnv(int env, string keyValue)
    {
      var keyValues = keyValue.Split(' ');
      int i;
      for (i=0; i < keyValues.Length; i++)
      {
        var vn = keyValues[i].Split(',');
        envpts[env,0,i] = double.Parse(vn[0]);
        if (!Fs.Equals(44100)) envpts[env,0,i] = envpts[env,0,i] / 44100 * Fs;
        envpts[env,1,i] = double.Parse(vn[1]);
      }
      envpts[env,0,keyValues.Length] = -1;
      envData[env,MAX] = envpts[env,0,keyValues.Length-1];
    }

    void Envy(long t, int g, bool OtN=false)
    {
      if(t < envData[g,NEXTT]) envData[g,ENV] = envData[g,ENV] + envData[g,dENV];
      else if (OtN)
      {
        if(t >= envData[g,MAX]) { envData[g,ENV] = 0; envData[g,dENV] = 0; envData[g,NEXTT] = 999999; }
        else { UpdateEnv(g, t); }
      }
      else UpdateEnv(g, t);
    }

    double waveform(double ph, long form)
    {
      return waveform(ph,(int)form);
    }
    double waveform(double ph, int form)
    {
      double w;
      
      switch (form)
      {
        case 0: //sine
          w = MathHelper.sin(ph % TwoPi);
          break;
        case 1: //sine^2
          w = MathHelper.fabs(2.0f * MathHelper.sin((0.5f * ph) % TwoPi)) - 1.0f; break;
        case 2: //tri
          while(ph < TwoPi) ph += TwoPi;
          w = 0.6366197f * (ph % TwoPi) - 1.0f;
          if(w>1.0f) w=2.0f-w;
          break;
        case 3: //saw
          w = ph - TwoPi * (double)(int)(ph / TwoPi);
          w = (0.3183098f * w) - 1.0f;
          break;
        default: //square
          w = (MathHelper.sin(ph % TwoPi) > 0.0) ? 1.0f : -1.0f;
          break;
      }
      return w;
    }
    
    #endregion

    public int do_ds2wav(string dsfile, string wavfile, int sampleRate=44100)
    {
      BUFFER_SIZE2 = BUFFER_SIZE * 2;
      DF  = new short[BUFFER_SIZE];
      phi = new double[BUFFER_SIZE];
      Fs=sampleRate;
      envpts = new double[8,3,EnvelopeCountMax];
      
      #region Vars
      #pragma warning disable 168, 219
      string sec, ver, comment, percent;

      // This is the precalculated length of our generated waveform data.
      long Length;
      long progress=0;
      long pos=0, tpos=0, tplus, totmp, t, i, j;

      double[] x = new double[3] ;
      double MasterTune, randmax, randmax2;
      int   MainFilter, HighPass;
      
      long  NON, NT, TON, DiON, TDroop=0, DStep;
      double a, b=0.0f, c=0.0f, d=0.0f, g, TT=0.0f, TTT, TL, NL;
      // Tone Freq 1 and 2
      double F1, F2, Fsync;
      double TphiStart=0.0f, Tphi, TDroopRate, ddF, DAtten, DGain;
      
      long  BON, BON2, BFStep, BFStep2, botmp;
      double BdF=0.0f, BdF2=0.0f, BPhi, BPhi2, BF, BF2, BQ, BQ2, BL, BL2;

      long  OON, OF1Sync=0, OF2Sync=0, OMode, OW1, OW2;
      double Ophi1, Ophi2, OF1, OF2, OL, Ot=0f, OBal1, OBal2, ODrive;
      double Ocf1, Ocf2, OcF, OcQ, OcA;
      double[,] Oc = new double[6,2];  //overtone cymbal mode
      double Oc0=0.0f, Oc1=0.0f, Oc2=0.0f;

      double MFfb, MFtmp, MFres, MFin=0.0f, MFout=0.0f;
      double DownAve;
      long  DownStart, DownEnd, jj;
      
      // check if algo is busy // leaving this here for now...
      // //sloppy programming? wait if DLL in use
      //while(busy==1) System.Threading.Thread.Sleep(0); busy=1;
      
      #pragma warning restore 168, 219
      
      if(wavemode==0) //semi-real-time adjustments if working in memory!!
      {
        mem_t = 1.0f; mem_o    = 1.0f; mem_n    = 1.0f;
        mem_b = 1.0f; mem_tune = 0.0f; mem_time = 1.0f;
      }
      //
      #endregion
      #region Variable Assignments
      if (DsPgm==null)
      {
        DsPgm=new DsPreset();
        var n=DsPgm.Load(dsfile);
        wavemode = 1;
        if (n!=0) { busy=0; return n; }
      }
      else wavemode = 0; // memory
      DsPgm = DsPgm ?? new DsPreset();
      
      getParseEnv(1,DsPgm.Tone.Envelope);
      getParseEnv(2,DsPgm.Noise.Envelope);
      getParseEnv(3,DsPgm.Overtones.Envelope1);
      getParseEnv(4,DsPgm.Overtones.Envelope2);
      getParseEnv(5,DsPgm.NoiseBand.Envelope);
      getParseEnv(6,DsPgm.NoiseBand2.Envelope);
      getParseEnv(7,DsPgm.General.FilterEnv);
      
      // ---------------------------------------------------
      // General
      // ---------------------------------------------------
      #region General
      
      comment = (DsPgm.General.Comment ?? " \0");
      if (comment.Length==0) comment=" \0";
      if (comment[comment.Length-1]!='\0') comment+="\0";
      
      //read MasterTuneter parameters
      
      timestretch = 0.01f * mem_time * DsPgm.General.Stretch;
      if(timestretch<0.2f) timestretch=0.2f;
      if(timestretch>10.0f) timestretch=10.0f;
      
      DGain = 1.0f; //leave this here!
      DGain = MathHelper.pow(10.0f, 0.05f * DsPgm.General.Level);
      MasterTune = DsPgm.General.Tuning;
      MasterTune = MathHelper.pow(1.0594631f, MasterTune + mem_tune);
      
      MainFilter = 2 * DsPgm.General.Filter;
      MFres = 0.0101f * DsPgm.General.Resonance;
      MFres = MathHelper.pow(MFres, 0.5f);
      HighPass = DsPgm.General.HighPass;
      
      #endregion
      // ---------------------------------------------------
      // Noise
      // ---------------------------------------------------
      #region Noise
      
      chkOn[1]   = DsPgm.Noise.On;
      sliLev[1]  = DsPgm.Noise.Level;
      NT         = DsPgm.Noise.Slope;
      NON = chkOn[1];
      NL = (double)(sliLev[1] * sliLev[1]) * mem_n;
      if (NT<0) { a = 1.0f + (NT / 105.0f); d = -NT / 105.0f; g = (1.0f + 0.0005f * NT * NT) * NL; }
      else
      {
        a = 1.0f;
        b = -NT / 50.0f;
        c = MathHelper.fabs((double)NT) / 100.0f;
        g = NL;
      }
      
      #endregion
      // --------------------------
      // if(InteropHelper.GetPrivateProfileInt(sec,"FixedSeq",0,dsfile)!=0)
      // MathHelper.srand(1); //fixed random sequence isn't enabled
      // ---------------------------------------------------
      // Tone
      // ---------------------------------------------------
      chkOn[0] = DsPgm.Tone.On; TON = chkOn[0];
      sliLev[0] = DsPgm.Tone.Level;
      TL = (double)(sliLev[0] * sliLev[0]) * mem_t;
      F1 = MasterTune * TwoPi * DsPgm.Tone.F1 / Fs;
      if(MathHelper.fabs(F1)<0.001f) F1=0.001f; //to prevent overtone ratio div0
      F2 = MasterTune * TwoPi * DsPgm.Tone.F2 / Fs;
      // Fsync = F2; // not referenced
      TDroopRate = (double)DsPgm.Tone.Droop;
      if(TDroopRate>0.0f)
      {
        TDroopRate = MathHelper.pow(10.0f, (TDroopRate - 20.0f) / 30.0f);
        TDroopRate = TDroopRate * -4.0f / envData[1,MAX];
        TDroop = 1;
        F2 = F1+((F2-F1)/(1.0f-MathHelper.exp(TDroopRate * envData[1,MAX])));
        ddF = F1 - F2;
      }
      else ddF = F2-F1;
      
      Tphi = (double)DsPgm.Tone.Phase / 57.29578f; //degrees>radians
      
      // ---------------------------------------------------
      // Overtones
      // ---------------------------------------------------
      
      chkOn[2]  = DsPgm.Overtones.On; OON = chkOn[2];
      sliLev[2] = DsPgm.Overtones.Level;
      OL = (double)(sliLev[2] * sliLev[2]) * mem_o;
      
      OMode = DsPgm.Overtones.Method;
      OF1 = MasterTune * TwoPi * DsPgm.Overtones.F1 / Fs;
      OF2 = MasterTune * TwoPi * DsPgm.Overtones.F2 / Fs;
      OW1 = DsPgm.Overtones.Wave1;
      OW2 = DsPgm.Overtones.Wave2;
      OBal2 = DsPgm.Overtones.Param;
      ODrive = MathHelper.pow(OBal2, 3.0f) / MathHelper.pow(50.0f, 3.0f);
      OBal2 *= 0.01f;
      OBal1 = 1.0f - OBal2;
      Ophi1 = Tphi;
      Ophi2 = Tphi;
      if(MainFilter==0) MainFilter = DsPgm.Overtones.Filter;
      if(DsPgm.Overtones.BTrack1 && (TON==1)) { OF1Sync = 1;  OF1 = OF1 / F1; }
      if(DsPgm.Overtones.BTrack2 && (TON==1)) { OF2Sync = 1;  OF2 = OF2 / F1; }

      OcA = 0.28f + OBal1 * OBal1;  //overtone cymbal mode
      OcQ = OcA * OcA;
      OcF = (1.8f - 0.7f * OcQ) * 0.92f; //multiply by env 2
      OcA *= 1.0f + 4.0f * OBal1;  //level is a compromise!
      Ocf1 = TwoPi / OF1;
      Ocf2 = TwoPi / OF2;
      for(i=0; i<6; i++) Oc[i,0] = Oc[i,1] = Ocf1 + (Ocf2 - Ocf1) * 0.2f * (double)i;
      // ---------------------------------------------------
      // NoiseBand
      // ---------------------------------------------------
      chkOn[3] = DsPgm.NoiseBand.On; BON = chkOn[3];
      sliLev[3] = DsPgm.NoiseBand.Level;
      BL = (double)(sliLev[3] * sliLev[3]) * mem_b;
      BF = MasterTune * TwoPi * DsPgm.NoiseBand.F / Fs;
      BPhi = TwoPi / 8.0f;
      BFStep = DsPgm.NoiseBand.dF;
      BQ = (double)BFStep;
      BQ = BQ * BQ / (10000.0f-6600.0f*(MathHelper.sqrt(BF)-0.19f));
      BFStep = 1 + (int)((40.0f - (BFStep / 2.5f)) / (BQ + 1.0f + (1.0f * BF)));
      // ---------------------------------------------------
      // NoiseBand2
      // ---------------------------------------------------
      chkOn[4] =  DsPgm.NoiseBand2.On; BON2 = chkOn[4];
      sliLev[4] = DsPgm.NoiseBand2.Level;
      BL2 = (double)(sliLev[4] * sliLev[4]) * mem_b;
      BF2 = MasterTune * TwoPi * DsPgm.NoiseBand2.F / Fs;
      BPhi2 = TwoPi / 8.0f;
      BFStep2 = DsPgm.NoiseBand2.dF;
      BQ2 = (double)BFStep2;
      BQ2 = BQ2 * BQ2 / (10000.0f - 6600.0f * (MathHelper.sqrt(BF2) - 0.19f));
      BFStep2 = 1 + (int)((40 - (BFStep2 / 2.5)) / (BQ2 + 1 + (1 * BF2)));
      // ---------------------------------------------------
      // Distortion
      // ---------------------------------------------------
      chkOn[5] = DsPgm.Distortion.On;
      DiON = chkOn[5];
      
      DStep = 1 + DsPgm.Distortion.Rate;
      if(DStep==7) DStep=20;
      if(DStep==6) DStep=10;
      if(DStep==5) DStep=8;

      clippoint = 32700;
      DAtten = 1.0f;

      if(DiON==1)
      {
        DAtten = DGain * LoudestEnv();
        if(DAtten>32700) clippoint=32700; else clippoint=(short)DAtten;
        DAtten = MathHelper.pow(2.0f, 2.0f * DsPgm.Distortion.Bits);
        DGain = DAtten * DGain * MathHelper.pow(10.0f, 0.05f * DsPgm.Distortion.Clipping);
      }
      // ---------------------------------------------------
      // Envelopes
      // ---------------------------------------------------
      randmax = 1.0f / MathHelper.RAND_MAX; randmax2 = 2.0f * randmax;
      for (i=1;i<8;i++) { envData[i,NEXTT]=0; envData[i,PNT]=0; }
      wavewords = 0;
      #endregion
      
      // ---------------------------------------------------
      // Generate
      // ---------------------------------------------------
      Length = LongestEnv();
      
      //if(wave!=NULL) free(wave);
      //wave = new short[2 * Length + 1280]; // We're using BinaryWriter wrapping a Stream
      //if(wave==NULL) {busy=0; return 3;}
      
      if (wavemode==1) {
        if (File.Exists(wavfile)) File.Delete(wavfile);
        mrStream = new FileStream(wavfile,FileMode.Create);
      } else {
        mrStream = new MemoryStream();
      }
      
      //string logfile = "log.log".Replace(".log",string.Format("{0}.log",Fs));
      //if (File.Exists(logfile)) File.Delete(logfile);
      //var logStream = new StreamWriter(File.OpenWrite(logfile));
      
      // getting prepared to do some multi-channel process
      var dswav = new DsWaveFile(comment,Length,Fs);
      using (var writer = new BinaryWriter(mrStream))
      {
        dswav.WriteHeader(writer); // write wave-header
        
        tpos = 0; // byte[] wave = new byte[BUFFER_SIZE];
        while(tpos < Length)
        {
          #region Generation Process Loop
          tplus = tpos + (BUFFER_SIZE - 1);
          if(NON==1) //noise
          {
            for(t=tpos; t<=tplus; t++)
            {
              Envy(t,2);
              
              x[2] = x[1];
              x[1] = x[0];
              x[0] = (randmax2 * MathHelper.rand()) - 1.0f;
              TT = a * x[0] + b * x[1] + c * x[2] + d * TT;
              
              DF[t - tpos] = MathHelper.lim(TT * g * envData[2,ENV]);
            }
            if(t>=envData[2,MAX]) NON=0;
          } //noise
          else for(j=0; j<BUFFER_SIZE; j++) DF[j]=0;
          
          if(TON==1) //tone
          {
            TphiStart = Tphi;
            if(TDroop==1) { for(t=tpos; t<=tplus; t++) phi[t - tpos] = F2 + (ddF * MathHelper.exp(t * TDroopRate)); }
            else {          for(t=tpos; t<=tplus; t++) phi[t - tpos] = F1 + (t / envData[1,MAX]) * ddF; }
            for(t=tpos; t<=tplus; t++)
            {
              totmp = t - tpos;
              Envy(t,1);
              
              Tphi = Tphi + phi[totmp];
              DF[totmp] = MathHelper.lim(
                DF[totmp] + (
                  TL * envData[1,ENV] * MathHelper.sin(
                    MathHelper.fmod(Tphi,TwoPi)
                   )
                 )
               );//overflow?
            }
            if(t>=envData[1,MAX]) TON=0;
          } //tone
          else for(j=0; j<BUFFER_SIZE; j++) phi[j]=F2; //for overtone sync
          
          if(BON==1) //noise band 1
          {
            for(t=tpos; t<=tplus; t++)
            {
              Envy(t,5);
              if((t % BFStep) == 0) BdF = randmax * MathHelper.rand() - 0.5f;
              BPhi = BPhi + BF + BQ * BdF;
              botmp = t - tpos;
              DF[botmp] = MathHelper.lim(DF[botmp] +MathHelper.cos(MathHelper.fmod(BPhi,TwoPi)) * envData[5,ENV] * BL);
            }
            if(t>=envData[5,MAX]) BON=0;
          } //noise band 1

          if(BON2==1) //noise band 2
          {
            for(t=tpos; t<=tplus; t++)
            {
              Envy(t,6);
              if((t % BFStep2) == 0) BdF2 = randmax * MathHelper.rand() - 0.5f;
              BPhi2 = BPhi2 + BF2 + BQ2 * BdF2;
              botmp = t - tpos;
              DF[botmp] = MathHelper.lim(DF[botmp] + MathHelper.cos(MathHelper.fmod(BPhi2,TwoPi)) * envData[6,ENV] * BL2);
            }
            if(t>=envData[6,MAX]) BON2=0;
          } //noise band 2

          for (t=tpos; t<=tplus; t++)
          {
            if(OON==1) //overtones
            {
              Envy(t,3,true);
              Envy(t,4,true);
              //
              TphiStart = TphiStart + phi[t - tpos];
              if(OF1Sync==1) Ophi1 = TphiStart * OF1; else Ophi1 = Ophi1 + OF1;
              if(OF2Sync==1) Ophi2 = TphiStart * OF2; else Ophi2 = Ophi2 + OF2;
              Ot=0.0f;
              switch (OMode)
              {
                case 0: //add
                  Ot = OBal1 * envData[3,ENV] * waveform(Ophi1, OW1);
                  Ot = OL * (Ot + OBal2 * envData[4,ENV] * waveform(Ophi2, OW2));
                  break;
                  
                case 1: //FM
                  Ot = ODrive * envData[4,ENV] * waveform(Ophi2, OW2);
                  Ot = OL * envData[3,ENV] * waveform(Ophi1 + Ot, OW1);
                  break;
                  
                case 2: //RM
                  Ot = (1 - ODrive / 8) + (((ODrive / 8) * envData[4,ENV]) * waveform(Ophi2, OW2));
                  Ot = OL * envData[3,ENV] * waveform(Ophi1, OW1) * Ot;
                  break;

                case 3: //808 Cymbal
                  for(j=0; j<6; j++)
                  {
                    Oc[j,0] += 1.0f;
                    
                    if(Oc[j,0]>Oc[j,1])
                    {
                      Oc[j,0] -= Oc[j,1];
                      Ot = OL * envData[3,ENV];
                    }
                  }
                  Ocf1 = envData[4,ENV] * OcF;  //filter freq
                  Oc0 += Ocf1 * Oc1;
                  Oc1 += Ocf1 * (Ot + Oc2 - OcQ * Oc1 - Oc0);  //bpf
                  Oc2 = Ot;
                  Ot = Oc1;
                  break;
                case 4: //subt (OL=Overtone Level)
                  // OBal1 is from our slider/mixer
                  Ot =            OBal1 * envData[3,ENV] * waveform(Ophi1, OW1);
                  Ot = OL * (Ot - OBal2 * envData[4,ENV] * waveform(Ophi2, OW2));
                  break;
              }
            } //overtones
            
            if (MainFilter==1) //filter overtones
            {
              Envy(t,7);
              MFtmp = envData[7,ENV];
              
              if(MFtmp >0.2f) MFfb = 1.001f - MathHelper.pow(10.0f, MFtmp - 1);
              else            MFfb = 0.999f - 0.7824f * MFtmp;
              
              MFtmp = Ot + MFres * (1.0f + (1.0f/MFfb)) * (MFin - MFout);
              MFin  = MFfb * (MFin  - MFtmp) + MFtmp;
              MFout = MFfb * (MFout - MFin)  + MFin;

              DF[t - tpos] = MathHelper.lim(DF[t - tpos] + (MFout - (HighPass * Ot)));
            } //filter overtones
            else if(MainFilter==2) //filter all
            {
              Envy(t,7);
              MFtmp = envData[7,ENV];

              if (MFtmp > 0.2f) MFfb = 1.001f - MathHelper.pow(10.0f, MFtmp - 1);
              else              MFfb = 0.999f - 0.7824f * MFtmp;

              //MFfb = MFfb / 44100.0f * Fs;

              MFtmp = DF[t - tpos] + Ot + MFres * (1.0f + (1.0f/MFfb)) * (MFin - MFout);
              MFin  = MFfb * (MFin  - MFtmp) + MFtmp;
              MFout = MFfb * (MFout - MFin)  + MFin;
              
              DF[t - tpos] = MathHelper.lim(MFout - (HighPass * (DF[t - tpos] + Ot)));
              
            } //filter all
            else DF[t - tpos] = MathHelper.lim(DF[t - tpos] + Ot); //no filter
            
          } //overtones + optional filtering
          
          if(DiON==1) //bit resolution
          {
            for(j=0; j<BUFFER_SIZE; j++)
            {
              DF[j] = MathHelper.lim(DGain * (int)(DF[j] / DAtten));
            }
            
            for(j=0; j<BUFFER_SIZE; j+=DStep) //downsampling
            {
              DownAve = 0; DownStart = j; DownEnd = j + DStep - 1;
              for(jj = DownStart; jj<=DownEnd; jj++) DownAve = DownAve + DF[jj];
              DownAve = DownAve / DStep;
              for(jj = DownStart; jj<=DownEnd; jj++) DF[jj] = (short)DownAve;
            }
          } //bit resolution
          else for(j=0; j<BUFFER_SIZE; j++) DF[j] = MathHelper.lim(DF[j] * DGain);
          
          #endregion
          for(j = 0; j<BUFFER_SIZE; j++) //clipping + output
          {
            wavewords++;
            if(DF[j] > clippoint)       writer.Write(clippoint); //wave[wavewords] = clippoint;
            else if(DF[j] < -clippoint) writer.Write((short)-clippoint); //wave[wavewords] = (short)-clippoint;
            else                        writer.Write(DF[j]); //wave[wavewords] = DF[j];
            
            //if(DF[j] > clippoint)       wave[wavewords] = clippoint;
            //else if(DF[j] < -clippoint) wave[wavewords] = (short)-clippoint;
            //else                        wave[wavewords] = DF[j];
          } //clipping + output
          // FIXME: REPORT PROGRESS // if(((100*tpos) / Length) > (progress + 4)) { progress = (100*tpos) / Length; sprintf(percent,"%i%%", progress); if(hWnd>0) SetWindowText(hWnd, percent); }
          tpos = tpos + BUFFER_SIZE;
        }
        //logStream.Close();
        //logStream.Dispose();
        //logStream = null;
        dswav.WriteTerminal(writer);
      }
      mrStream.Dispose();
      mrStream = null;
      return 0;
    }
    
    // return true on error
    static public bool DsGenWaveform(string dsFile, int sampleRate=44100)
    {
      string ext = null;
      var d2w = new ds2wav();
      var inputfile = Path.GetFileName(dsFile);

      ext = Path.GetExtension(inputfile);
      if (ext.ToLower() != ".ds") return false;

      var outputfile = GetOutFile(dsFile,sampleRate);
      int result = d2w.do_ds2wav(dsFile,outputfile,sampleRate);
      d2w = null;
      return result != 0;
    }
    
    static bool IsExtension(string inputfile, string extIn = ".ds")
    {
      return Path.GetExtension(inputfile).ToLower() == extIn;
    }
    
    internal static string GetOutFile(string inputfile, int sampleRate, string extIn = ".ds", string extOut=".wav")
    {
      var ext = Path.GetExtension(inputfile);
      if (ext.ToLower() != extIn) return null;
      var outputfile = inputfile.Replace(ext,extOut);
      if (sampleRate!=44100) outputfile = outputfile.Replace(extOut,string.Format("-{0}.wav",sampleRate));
      return outputfile;
    }

  }
}
