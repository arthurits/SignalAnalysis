# SignalAnalysis changelog

## SignalAnalysis 1.6
* Target .NET 9.0.
* Update [FftSharp](https://github.com/swharden/FftSharp) api to version 2.2.0.
* Update Norwegian (Bokm�l) (nb-NO) translation. _Thanks @bjartelund_.
* Update Russian translation. _Thanks @ shmudivel_.
* [ScottPlot](https://github.com/ScottPlot/ScottPlot) control has been updated to version 4.1.74.
* Add paralellization for entropy and fractal dimension.
* Add ratio Shannon/Ideal entropy.
* Add ApEn and SampEn algorithm choice.
* Add Bluestein option to compute FFT for arrays of any length.
* Correct not-casted-to-double division bug in fractal dimension.
* Update text and bin file formats.

## SignalAnalysis 1.5
* Add variance computation.
* Add box plot computation as an user-selected option.
* Update time-related (days, hours, minutes, seconds, and miliseconds) grammar issues for all cultures/languages
* [ScottPlot](https://github.com/ScottPlot/ScottPlot) control has been updated to version 4.1.63.
* Minor bug corrections.

## SignalAnalysis 1.4
* Add Simplified Chinese (zh-Hans) translation. _Thanks @myd7349_.
* Add numerical differentiation.
* Add numerical integration.
* [ScottPlot](https://github.com/ScottPlot/ScottPlot) control has been updated to version 4.1.61.
* Minor bug corrections.

## SignalAnalysis 1.3
* Target .NET 7.

## SignalAnalysis 1.2
* Multilanguage support:
  * German (de-DE). _Thanks @m1ga and @Senuros_.
  * Norwegian (nb-NO). _Thanks @rubjo_.
  * Bangla (bn-BD). _Thanks @sarequl_.
  * Danish (da-DK). _Thanks @afskylia_.
  * Hindi (hi-IN). _Thanks @fadkeabhi_.
  * Italian (it-IT). _Thanks @Pyr0x1_.
  * Arabic (ar). _Thanks @mazen-r_.
  * Portuguese (pt-BR). _Thanks @lhardt_.
  * Russian (ru-RU). _Thanks @Ujjwal-soni98_.
  * Lithuanian (lt-LT). _Thanks @mantasio_.
  * Dutch (nl-BE). _Thanks @cvc04_.
  * Turkish (tr-TR). _Thanks @faisalahammad_.
* [ScottPlot](https://github.com/ScottPlot/ScottPlot) control has been updated to version 4.1.59.
* Corrections related to reading files, UI controls, and showing total time in status bar.

## SignalAnalysis 1.1
This release features the following improvements:
* String resources are accessed from a static class.
* Application icon has been simplified for small size.
* Template files are named correctly according to the culture.
* [ScottPlot](https://github.com/ScottPlot/ScottPlot) control has been updated to version 4.1.51.

## SignalAnalysis 1.0
Initial release. Options include:
* Compatible with *.elux, *.sig, *.txt, and *.bin custom file formats.
* Computes the FFT (data is reduced to the lowest 2n value).
* Computes the fractal dimension.
* Computes entropy values.
* Multilanguage (English and Spanish) UI.
* Splash screen launcher created in MASM x64.
