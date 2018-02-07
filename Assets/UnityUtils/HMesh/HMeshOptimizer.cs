using System.Collections.Generic;
using Priority_Queue;

/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

// C# port of https://github.com/janba/GEL/blob/master/src/GEL/HMesh/mesh_optimization.h

public abstract class EnergyFun
{
    public abstract double DeltaEnergy(Halfedge h);

    public virtual double Energy(Halfedge h) {return 0;}
}

public class MinAngleEnergy : EnergyFun {

    double thresh;


    public MinAngleEnergy(double thresh)
    {
        this.thresh = thresh;
    }

    public double MinAngle(Vector3D v0, Vector3D v1, Vector3D v2)
    {
        Vector3D a = (v1-v0).normalized;
        Vector3D b = (v2-v1).normalized;
        Vector3D c = (v0-v2).normalized;

        return System.Math.Min(Vector3D.Dot(a,-c), System.Math.Min(Vector3D.Dot(b,-a), Vector3D.Dot(c,-b)));
    }

    public override double DeltaEnergy(Halfedge h)
    {
        // Walker w = m.walker(h);

        var hv = h.vert;
        var hnv = h.next.vert;
        var hov= h.opp.vert;
        var honv = h.opp.next.vert;

        Vector3D v0 = hv.positionD;
        Vector3D v1 = hnv.positionD;
        Vector3D v2 = hov.positionD;
        Vector3D v3 = honv.positionD;

        Vector3D n1a = Vector3D.Cross(v1-v0,v3-v0).normalized;
        Vector3D n2a = Vector3D.Cross(v3-v2,v1-v2).normalized;

        if(Vector3D.Dot(n1a, n2a) > thresh)
        {
            double before = System.Math.Min(MinAngle(v0,v1,v2), MinAngle(v0,v2,v3));
            double after = System.Math.Min(MinAngle(v0,v1,v3), MinAngle(v1,v2,v3));
            return -(after-before);
        }
        return double.MaxValue;

    }
}

public class DihedralEnergy : EnergyFun {

    double gamma;
    bool use_alpha;

    public DihedralEnergy(double _gamma = 4.0, bool _use_alpha = false)
    {
        gamma=_gamma;
        use_alpha = _use_alpha;
    }

    public override double DeltaEnergy(Halfedge h)
    {
        compute_angles(h);

        var hv = h.vert;
        var hov = h.opp.vert;
        var hnv = h.next.vert;
        var honv = h.opp.next.vert;

        var va = hv.positionD;
        var vb = hov.positionD;
        var vc = hnv.positionD;
        var vd = honv.positionD;

        if(use_alpha){
            double before =
                edge_alpha_energy(va,vb,ab_12)
                +edge_alpha_energy(va,vc,ab_a1)
                +edge_alpha_energy(vc,vb,ab_b1)
                +edge_alpha_energy(vd,vb,ab_2c)
                +edge_alpha_energy(vd,va,ab_2d);

            double after =
                edge_alpha_energy(vd,vc,aa_12)
                +edge_alpha_energy(vb,vc,aa_b1)
                +edge_alpha_energy(vd,vb,aa_c1)
                +edge_alpha_energy(va,vc,aa_2a)
                +edge_alpha_energy(vd,va,aa_2d);

            return (after-before);
        } else {

            double before =
                edge_c_energy(va,vb,ab_12)
                +edge_c_energy(va,vc,ab_a1)
                +edge_c_energy(vc,vb,ab_b1)
                +edge_c_energy(vd,vb,ab_2c)
                +edge_c_energy(vd,va,ab_2d);

            double after =
                edge_c_energy(vd,vc,aa_12)
                +edge_c_energy(vb,vc,aa_b1)
                +edge_c_energy(vd,vb,aa_c1)
                +edge_c_energy(va,vc,aa_2a)
                +edge_c_energy(vd,va,aa_2d);

            return after-before;
        }
    }

    public override double Energy(Halfedge h)
    {
        var hf = h.face;
        var hof = h.opp.face;

        double a = cos_ang(hf.GetNormal(), hof.GetNormal());

        var hv = h.vert;
        var hov = h.opp.vert;

        var va = hv.positionD;
        var vb = hov.positionD;

        if(use_alpha)
            return edge_alpha_energy(va,vb,a);

        return edge_c_energy(va,vb,a);
    }

    double min_angle( Halfedge h)
    {
        compute_angles(h);
        return System.Math.Min(System.Math.Min(System.Math.Min(System.Math.Min(aa_12, aa_b1), aa_c1), aa_2a), aa_2d);
    }

    double cos_ang(Vector3D n1, Vector3D n2)
    {
        return System.Math.Max(-1.0, System.Math.Min(1.0, Vector3D.Dot(n1, n2)));
    }

    double edge_alpha_energy(Vector3D v1, Vector3D v2, double ca)
    {
        return System.Math.Pow((v1-v2).magnitude*(System.Math.Acos(ca)), 1.0f/gamma);
    }

    double edge_c_energy(Vector3D v1, Vector3D v2, double ca)
    {
        return System.Math.Pow((v1-v2).magnitude*(1-ca), 1.0f/gamma);
    }

    void compute_angles(Halfedge h)
    {
        var hv = h.vert;
        var hov = h.opp.vert;
        var hnv = h.next.vert;
        var honv = h.opp.next.vert;

        var va = hv.positionD;
        var vb = hov.positionD;
        var vc = hnv.positionD;
        var vd = honv.positionD;

        var fa = (h.next.opp==null) ? null : h.next.opp.face;
        var fb = (h.next.next.opp==null) ? null : h.next.next.opp.face;
        var fc = (h.opp == null || h.opp.next.opp == null)?null:h.opp.next.opp.face;
        var fd = (h.opp == null || h.opp.next.next.opp == null)?null: h.opp.next.next.opp.face;

        var n1 = (Vector3D.Cross(vc-va, vb-va)).normalized;
        var n2 = (Vector3D.Cross(vb-va, vd-va)).normalized;

        var na = fa == null ? Vector3D.zero : fa.GetNormal();
        var nb = fb == null ? Vector3D.zero : fb.GetNormal();
        var nc = fc == null ? Vector3D.zero : fc.GetNormal();
        var nd = fd == null ? Vector3D.zero : fd.GetNormal();


        var fn1 = Vector3D.Cross(vb-vc, vd-vc).normalized;
        var fn2 = Vector3D.Cross(vd-vc, va-vc).normalized;

        ab_12 = cos_ang(n1,n2);
        ab_a1 = cos_ang(na,n1);
        ab_b1 = cos_ang(nb,n1);
        ab_2c = cos_ang(n2,nc);
        ab_2d = cos_ang(n2,nd);

        aa_12 = cos_ang(fn1,fn2);
        aa_b1 = cos_ang(nb,fn1);
        aa_c1 = cos_ang(nc, fn1);
        aa_2a = cos_ang(fn2, na);
        aa_2d = cos_ang(fn2,nd);
    }


    double ab_12;
    double ab_a1;
    double ab_b1;
    double ab_2c;
    double ab_2d;

    double aa_12;
    double aa_b1;
    double aa_c1;
    double aa_2a;
    double aa_2d;
}

public class ValencyEnergy : EnergyFun
{
    int sqr(int x)
    {
        return x * x;
    }

    public override double DeltaEnergy(Halfedge h)
    {
        // Walker w = m.walker(h);

        Vertex v1 = h.opp.vert;
        Vertex v2 = h.vert;
        Vertex vo1 = h.next.vert;
        Vertex vo2 = h.opp.next.vert;

        int val1  = v1.Valency;
        int val2  = v2.Valency;
        int valo1 = vo1.Valency;
        int valo2 = vo2.Valency;

        // The optimal valency is four for a boundary vertex
        // and six elsewhere.
        int t1  = v1.IsBoundary() ? 4 : 6;
        int t2  = v2.IsBoundary() ? 4 : 6;
        int to1 = vo1.IsBoundary() ? 4 : 6;
        int to2 = vo2.IsBoundary() ? 4 : 6;

        int before =
            sqr(val1-t1)+sqr(val2-t2)+
            sqr(valo1-to1)+sqr(valo2-to2);
        int after =
            sqr(valo1+1-to1)+sqr(val1-1-t1)+
            sqr(val2-1-t2)+sqr(valo2+1-to2);

        return after-before;
    }
}

public class PQElement
{
    public double pri;
    public Halfedge h;
    public int time;

    //PQElement() {}
    public PQElement(double _pri, Halfedge _h, int _time)
    {
        pri = _pri;
        h = _h;
        time = _time;
    }
};

public class HalfEdgeCounter {
    public int touched;
    public bool isRemovedFromQueue;
};

public class HMeshOptimizer
{
    public bool FaceLabelContrain = true;
    public bool FaceNormalContrain = true;
    // angle in degrees
    public double epsilonAngle = 0.1f;

    /// Optimize in a greedy fashion.
    public void PriorityQueueOptimization(HMesh m, EnergyFun efun)
    {
        //HalfEdgeAttributeVector<HalfEdgeCounter> counter(m.allocated_halfedges(), HalfEdgeCounter{0, false});
//        VertexAttributeVector<int> flipCounter(m.allocated_vertices(), 0);
        Dictionary<int, HalfEdgeCounter> counter = new Dictionary<int, HalfEdgeCounter>();
        Dictionary<int, int> flipCounter = new Dictionary<int, int>();
        Priority_Queue.SimplePriorityQueue<PQElement> Q = new SimplePriorityQueue<PQElement>();
        //priority_queue<PQElement> Q;

        //cout << "Building priority queue"<< endl;
        int time=1;
        foreach (var h in m.GetHalfedgesRaw()){
            if (!counter.ContainsKey(h.id))
            {
                counter.Add(h.id, new HalfEdgeCounter());
            }
            AddToQueue(counter, Q, h, efun, flipCounter, time);
        }

        //cout << "Emptying priority queue of size: " << Q.size() << " ";
        while(Q.Count>0)
        {

            PQElement elem = Q.Dequeue();

            //Walker w = m.walker(elem.h);

            if(counter[elem.h.id].isRemovedFromQueue) // if item already has been processed continue
                continue;

            counter[elem.h.id].isRemovedFromQueue = true;

            if(counter[elem.h.id].touched != elem.time) {
                if (efun.DeltaEnergy(elem.h) >= 0) {
                    continue;
                }
            }
            if(!PrecondFlipEdge(elem.h))
                continue;

            flipCounter[elem.h.vert.id]++;

            elem.h.Flip();

            AddOneRingToQueue(counter, Q, elem.h.vert, efun, flipCounter, time);
            AddOneRingToQueue(counter, Q, elem.h.next.vert, efun, flipCounter, time);
            AddOneRingToQueue(counter, Q, elem.h.opp.vert, efun, flipCounter, time);
            AddOneRingToQueue(counter, Q, elem.h.opp.next.vert, efun, flipCounter, time);

        }
    }

    void AddOneRingToQueue(Dictionary<int, HalfEdgeCounter> counter, SimplePriorityQueue<PQElement> Q, Vertex v, EnergyFun efun, Dictionary<int, int> flipCounter, int time)
    {

        foreach (var he in v.Circulate()){
            AddToQueue(counter, Q, he, efun, flipCounter, time);
        }
    }

    private void AddToQueue(Dictionary<int, HalfEdgeCounter> counter, SimplePriorityQueue<PQElement> Q, Halfedge h, EnergyFun efun, Dictionary<int, int> flipCounter, int time)
    {
        if (h.IsBoundary())
        {
            return;
        }
        // only consider one of the halfedges
        if (h.id < h.opp.id)
        {
            h = h.opp;
        }
        // if half edge already tested for queue in the current frame then skip
        HalfEdgeCounter c = null;
        if (!counter.TryGetValue(h.id, out c))
        {
            c = new HalfEdgeCounter();
            counter[h.id] = c;
        }
        if (c.touched == time){
            return;
        }
        c.isRemovedFromQueue = false;

        if(!PrecondFlipEdge(h))
            return;

        double energy = efun.DeltaEnergy(h);
        c.touched = time;

        const int avgValence = 6;
        int count = 0;
        if (!flipCounter.TryGetValue(h.vert.id, out count))
        {
            flipCounter[h.vert.id] = 0;
        }
        if((energy < 0) && (count < avgValence)){
            Q.Enqueue(new PQElement(energy, h, time),(float)energy);
        }
    }

    bool PrecondFlipEdge(Halfedge h)
    {
        Face hf = h.face;
        if (h.opp == null)
        {
            return false;
        }
        Face hof = h.opp.face;

        if (FaceLabelContrain){
            if (hf.label != hof.label)
            {
                return false;
            }
        }
        if (FaceNormalContrain)
        {
            var fn = hf.GetNormal();
            var fon = hof.GetNormal();

            if (Vector3D.Angle(fn, fon) > epsilonAngle)
            {
                return false;
            }
        }

        // boundary case
        if(hf == null || hof == null)
            return false;

        // We can only flip an edge if both incident polygons are triangles.
        if(hf.Circulate().Count != 3 || hof.Circulate().Count !=3)
            return false;


        // non boundary vertices with a valency of less than 4(less than 3 after operation) degenerates mesh.
        Vertex hv = h.vert;
        var hov = h.opp.vert;

        if ((hv.Valency < 4 && !hv.IsBoundary()) || (hov.Valency < 4 && !hov.IsBoundary())){
            return false;
        }

        // Disallow flip if vertices being connected already are.
        Vertex hnv = h.next.vert;
        Vertex honv = h.opp.next.vert;

        if(hnv.GetSharedEdge(honv) != null){
            return false;
        }

        return true;
    }

    /// Minimize the angle between adjacent triangles. Almost the same as mean curvature minimization
    public void MinimizeDihedralAngle(HMesh m, int max_iter = 10000, bool alpha = false,
        double gamma = 4.0)
    {
        DihedralEnergy energy_fun = new DihedralEnergy(gamma, alpha);
        PriorityQueueOptimization(m, energy_fun);
    }

    /// Maximizes the minimum angle of triangles. Makes the mesh more Delaunay.
    public void MaximizeMinAngle(HMesh m, float thresh)
    {
        MinAngleEnergy energy_fun = new MinAngleEnergy (thresh);
        PriorityQueueOptimization(m, energy_fun);
    }

    /// Tries to achieve valence 6 internally and 4 along edges.
    public void OptimizeValency(HMesh m)
    {
        ValencyEnergy energy_fun = new ValencyEnergy();
        PriorityQueueOptimization(m, energy_fun);
    }
}
