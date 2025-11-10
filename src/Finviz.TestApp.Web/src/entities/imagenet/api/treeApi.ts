import { api } from "@/shared/api/httpClient";
import type { ImageNetNode } from "../model/types";

export async function getRootNodes(): Promise<ImageNetNode[]> {
    const { data } = await api.get<ImageNetNode[]>("/api/imagenet/tree/root");
    return data;
}

export async function getNodeChildren(parentId: number): Promise<ImageNetNode[]> {
    const { data } = await api.get<ImageNetNode[]>(
        `/api/imagenet/tree/children?parentId=${parentId}`
    );
    return data;
}
