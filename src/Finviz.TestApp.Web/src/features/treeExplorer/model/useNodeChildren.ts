import { useQuery } from "@tanstack/react-query";
import { getRootNodes, getNodeChildren } from "@/entities/imagenet/api/treeApi";
import type { ImageNetNode } from "@/entities/imagenet/model/types";

export function useNodeChildren(parentId: number | "root") {
    return useQuery<ImageNetNode[]>({
        queryKey: ["node-children", parentId],
        queryFn: () =>
            parentId === "root" ? getRootNodes() : getNodeChildren(parentId as number),
        staleTime: 60_000
    });
}
