using UnityEngine;
using Assets.Lattice;

public class IcoDebug : MonoBehaviour {
  private Lattice<Vector3> lattice;

  void Start() {
    lattice = new Lattice<Vector3>(new IcoLattice(5));
  }

  void Update() {
    foreach (var edge in lattice.Edges) {
      Debug.DrawLine(transform.TransformPoint(lattice.Vertices[edge.a]), transform.TransformPoint(lattice.Vertices[edge.b]));
    }
  }
}
