
ds2wav (c#)
==============

M.D.A. DrumSynth v2.0, translated to csharp.

- http://mda.smartelectronix.com/drumsynth.htm
- https://sourceforge.net/projects/drumsynth


----

The idea here was to create a 'module' to assist in automating some batch
process.  Say you come up with a XML, json or YAML scheme that encapsulates
several DS programs assigning them keys and/or key-ranges within a drum-kit
or instrument sfz you intend to export (or something)

I suspect future elaboration to 'pack' and 'unpack' a directory or perhaps
something along the terms of a `*.dsb` or `*.ds-bank`, kit or instrument
preset(s).

WANT
------------

I hope this (C#6) compiles in MAC and/or X-nix.

See: https://developer.xamarin.com/guides/cross-platform/advanced/csharp_six/#Mac

USE
-------------

Drag-drop a directory or `*.ds` program into the executable (or shortcut to
the app) to generate waveforms.

or via command-line:

    ds2wav [path]

Where `path` is either a `*.ds` program (file) or a directory containing
multiple `*.ds` programs—a 16-bit mono 44.1 kbps PCM R-IFF/WAVE sample
will be generated alongside the `*.ds`.

    ds2wav [ds_file] [wave_file]

Where `ds_file` is a path to a valid `*.ds` file and `wave_file` is the
projected wave-file output.

DEV
-------------

TARGET .NET Framework v4.0; MSBuild 5.0; C# v6.0 auto-property initializers
are used among a few other C#6 use-cases.

```csharp
class foo { public int bar { get; set; } = -32768; }
```

(just sayin')

SharpDevelop v5.x or perhaps Xamarin Studio (monodevelop) would probably the
fastest, easiest way to build, currently.

The SLN file is in the `./ide` directory, so simply load it in Xamarin,
SharpDevelop or your IDE of choice, set your targets and build (provided it
can handle c#6).


    DrumSynth DS file renderer is
    Copyright (c)1998-2000 Paul Kellett (mda-vst.com)
    Version 2.0 (5/10/00)

    2016 CSharp translation - github.com/tfwio

