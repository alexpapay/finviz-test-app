import { api } from "@/shared/api/httpClient";
import type { ImageNetNode } from "../model/types";

const TREE_ENDPOINT = "/api/imagenet/tree";

export const getRootNodes = async (): Promise<ImageNetNode[]> => {
    try {
        const { data } = await api.get<ImageNetNode[]>(`${TREE_ENDPOINT}/root`);
        return data;
    } catch (error) {
        return [];
    }
};

export const getNodeChildren = async (
    parentId: number
): Promise<ImageNetNode[]> => {
    try {
        const { data } = await api.get<ImageNetNode[]>(
            `${TREE_ENDPOINT}/children/${parentId}`
        );
        return data;
    } catch (error) {
        return [];
    }
};
