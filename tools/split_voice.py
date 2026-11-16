"""Unit voice lines: splits each recorded set into single lines for the game.

  python tools/split_voice.py

Each file art_pack/voice/units/<crew>_<kind>.mp3 (made with ElevenLabs, several short lines with long pauses
between them) is cut at its pauses into game/Assets/Art/Audio/Voice/<crew>_<kind>_<n>.ogg.
  crew: pilot (fighters, bombers, scouts), bridge (frigates, destroyers), capital (Capital, Super-Capital)
  kind: select, move, attack
Needs ffmpeg on the PATH.
"""
import glob
import os
import re
import subprocess

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "art_pack", "voice", "units")
OUT = os.path.join(ROOT, "game", "Assets", "Art", "Audio", "Voice")
PAD = 0.06  # seconds of pause kept either side of a line


def pauses(path):
    log = subprocess.run(["ffmpeg", "-hide_banner", "-i", path, "-af", "silencedetect=noise=-35dB:d=0.35", "-f", "null", "-"],
                         capture_output=True, text=True).stderr
    starts = [float(x) for x in re.findall(r"silence_start: ([0-9.]+)", log)]
    ends = [float(x) for x in re.findall(r"silence_end: ([0-9.]+)", log)]
    dur = float(re.search(r"Duration: (\d+):(\d+):([0-9.]+)", log).group(3)) + \
        60 * float(re.search(r"Duration: (\d+):(\d+):", log).group(2))
    return list(zip(starts, ends)), dur


SQUELCH = 0.08  # seconds of static burst before and after each line (the radio keying open and closed)


def radio(src, a, b, out):
    """Cuts src[a:b] and makes it sound like a ship-to-ship radio: telephone band, squashed and slightly
    crunchy, a low static hiss throughout, and a burst of static as the channel opens and closes."""
    length = (b - a) + 2 * SQUELCH
    voice = (f"[0:a]atrim=start={a:.3f}:end={b:.3f},asetpts=PTS-STARTPTS,aformat=sample_rates=44100:channel_layouts=mono,"
             f"highpass=f=320,lowpass=f=3300,acompressor=threshold=0.08:ratio=8:attack=5:release=60:makeup=2.2,"
             f"acrusher=bits=9:mix=0.35:mode=log,adelay={int(SQUELCH * 1000)},apad=whole_dur={length:.3f}[v]")
    static = (f"anoisesrc=color=white:amplitude=1:sample_rate=44100:duration={length:.3f},highpass=f=900,lowpass=f=6500,"
              f"volume='if(lt(t,{SQUELCH})+gt(t,{length - SQUELCH:.3f}),0.16,0.022)':eval=frame[n]")
    subprocess.run(["ffmpeg", "-hide_banner", "-loglevel", "error", "-y", "-i", src,
                    "-filter_complex", f"{voice};{static};[v][n]amix=inputs=2:duration=longest:normalize=0,alimiter=limit=0.95",
                    "-ac", "1", "-c:a", "libvorbis", "-q:a", "5", out], check=True)


def main():
    os.makedirs(OUT, exist_ok=True)
    for f in sorted(glob.glob(os.path.join(SRC, "*.mp3"))):
        name = os.path.splitext(os.path.basename(f))[0]
        gaps, dur = pauses(f)
        # Lines are what lies between the pauses (a pause at the very start or end is just trimmed).
        cuts, t = [], 0.0
        for s, e in gaps:
            if s - t > 0.15:
                cuts.append((t, s))
            t = e
        if dur - t > 0.15:
            cuts.append((t, dur))
        for old in glob.glob(os.path.join(OUT, name + "_*.ogg")):
            os.remove(old)
        for i, (a, b) in enumerate(cuts, 1):
            a, b = max(0.0, a - PAD), min(dur, b + PAD)
            radio(f, a, b, os.path.join(OUT, f"{name}_{i}.ogg"))
        print(f"{name}: {len(cuts)} lines")


if __name__ == "__main__":
    main()
