/// UnityUtils https://github.com/mortennobel/UnityUtils
/// By Morten Nobel-Jørgensen
/// License lgpl 3.0 (http://www.gnu.org/licenses/lgpl-3.0.txt)

public struct IntPair  : System.IEquatable<IntPair>{
	public readonly int first;
	public readonly int second;
	public IntPair(int first, int second){
		this.first = first;
		this.second = second;
	}

	public override bool Equals (object obj)
	{
		return base.Equals (obj);
	}
	
	public override int GetHashCode ()
	{
		return first ^ second;
	}
	
	bool System.IEquatable<IntPair>.Equals(IntPair obj){
		return first == obj.first && second == obj.second;
	}
}
