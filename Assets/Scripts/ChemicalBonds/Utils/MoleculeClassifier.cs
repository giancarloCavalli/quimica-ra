using System.Collections.Generic;
using UnityEngine;

public class MoleculeClassifier
{
  public static Molecule GetMoleculeBasedOn(Transform transform, List<string> atomsBonded)
  {
    return transform.name switch
    {
      "Oxigen" => GetMoleculeBasedOnOxigen(atomsBonded),
      "Chlorine" => GetMoleculeBasedOnChlorine(atomsBonded),
      _ => Molecule.None,
    };
  }

  private static Molecule GetMoleculeBasedOnOxigen(List<string> atomsBonded)
  {
    if (atomsBonded.Contains("Hidrogen1") && atomsBonded.Contains("Hidrogen2"))
    {
      return Molecule.H2O;
    }
    else if (atomsBonded.Contains("Sodium") && (atomsBonded.Contains("Hidrogen1") || atomsBonded.Contains("Hidrogen2")))
    {
      return Molecule.NaOH;
    }
    else
    {
      return Molecule.None;
    }
  }

  private static Molecule GetMoleculeBasedOnChlorine(List<string> atomsBonded)
  {
    if (atomsBonded.Contains("Sodium"))
    {
      return Molecule.NaCl;
    }
    else if (atomsBonded.Contains("Hidrogen1") || atomsBonded.Contains("Hidrogen2"))
    {
      return Molecule.HCl;
    }
    else
    {
      return Molecule.None;
    }
  }
}
