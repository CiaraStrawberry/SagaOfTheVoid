namespace Apex.LoadBalancing;

public class NavLoadBalancer : LoadBalancer
{
	public static readonly ILoadBalancer steering = new LoadBalancedQueue(10);

	public static readonly ILoadBalancer dynamicObstacles = new LoadBalancedQueue(10);

	public static readonly ILoadBalancer scanners = new LoadBalancedQueue(10, 1f, 20, 2);

	private NavLoadBalancer()
	{
	}
}
