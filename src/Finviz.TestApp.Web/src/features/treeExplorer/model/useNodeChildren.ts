import { useQuery, keepPreviousData } from "@tanstack/react-query";
import { getRootNodes, getNodeChildren } from "@/entities/imagenet/api/treeApi";
import type { ImageNetNode } from "@/entities/imagenet/model/types";
import { DEFAULT_CACHE_TTL } from "@/shared/config/constants";

export const useNodeChildren = (parentId: number | "root") =>
    useQuery<ImageNetNode[], Error>({
        queryKey: ["node-children", parentId],
        queryFn: () =>
            parentId === "root"
                ? getRootNodes()
                : getNodeChildren(parentId as number),
        staleTime: DEFAULT_CACHE_TTL,
        placeholderData: keepPreviousData,
    });
