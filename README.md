# SdvBench: Microsoft SDV Benchmarks

## Coming Soon!

This repository contains a subset of the internal tests used by Microsoft's [Static Driver Verifier](https://docs.microsoft.com/en-us/windows-hardware/drivers/devtest/static-driver-verifier) tool. These tests are generated from Window's Device Drivers while checking for one of the various [properties](https://msdn.microsoft.com/en-us/library/windows/hardware/ff551714) that WDM drivers must satisfy. These represent one of the most comprehensive and challenging collection of verification benchmarks. 

Each benchmark is a [Boogie](https://github.com/boogie-org/boogie) file and represents checking of one property on a single driver. The property is already instrumented into the code as assertions. There is a well-defined entrypoint, which is the unique procedure tagged with `{:entrypoint}` annotation. The goal of the verifier is to find an execution of the program, starting at this entrypoint, that ends in an assertion failure. The entire process of how the file is generated is explained in our [FSE 2014](https://www.microsoft.com/en-us/research/publication/powering-the-static-driver-verifier-using-corral/) paper. 

The programs are readilty analyzed by [Corral](https://www.microsoft.com/en-us/research/project/q-program-verifier/), a whole-program verifier for Boogie programs. The flags used to run Corral are mentioned alongside each file.

These program carry some preculiarities specific of the use of Corral inside SDV. For instance, all loops have been converted to recursive procedures. Predicates that are used as Houdini candidates (for generating program invariants) are detailed alongside the procedure tagged with the annotation `{:template}`. We invite contributions from the community to help navigate these benchmarks and make them accessible to other verification tools.

## Contact

Please use github issues for questions and comments. We are looking forward to hearing from you.
